using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Categories.DTOs;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using MediatR;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Categories.CQRS.Queries;

public class GetCategory
{
    public sealed record Query(string UserId, string CategoryId, string ChildCategoryId = "") : ICategoriesQuery<Result<CategoryDto>>;

    public sealed class Handler(ICategoriesRepository repository) : IRequestHandler<Query, Result<CategoryDto>>
    {
        private readonly ICategoriesRepository _repository = repository ??
                                                              throw new ArgumentNullException(nameof(repository));

        public async Task<Result<CategoryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validator = new Validator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            var parentCategoryResult = await _repository.GetByIdAsync(request.UserId, request.CategoryId, cancellationToken);

            if (parentCategoryResult.IsFailed)
                return Result.Fail(parentCategoryResult.Errors);

            var parentCategoryDocument = parentCategoryResult.ValueOrDefault;

            if (parentCategoryDocument is null)
                return Result.Fail("Could not find the Category");

            if (string.IsNullOrWhiteSpace(request.ChildCategoryId))
            {
                var parentCategoryDto = Mappers.DocumentToDtoMappings.Map(parentCategoryResult.Value);

                if (parentCategoryDto is null)
                    throw new InvalidOperationException(
                        $"Could not map the CategoryDocument '{parentCategoryResult.Value.Id} to its DTO");

                return parentCategoryDto;
            }

            if (!ObjectId.TryParse(request.ChildCategoryId, out var childObjectId))
                return Result.Fail($"Invalid Child CategoryId: `{request.ChildCategoryId}`");

            var childCategoryDocument = parentCategoryDocument.SubCategories.Find(s => s.Id.Equals(childObjectId));

            if (childCategoryDocument is null)
                return Result.Fail($"Could not find the Child Category with Id: `{request.ChildCategoryId}`");

            var childCategoryDto = Mappers.DocumentToDtoMappings.Map(childCategoryDocument);

            if (childCategoryDto is null)
                throw new InvalidOperationException(
                    $"Could not map the CategoryDocument '{childCategoryDocument.Id} to its DTO");

            return childCategoryDto;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}