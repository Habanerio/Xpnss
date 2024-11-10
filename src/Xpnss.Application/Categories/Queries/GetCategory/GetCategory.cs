using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.Categories.DTOs;
using Habanerio.Xpnss.Application.Mappers;
using Habanerio.Xpnss.Domain.Categories.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Application.Categories.Queries.GetCategory;

public sealed record GetCategoryQuery(string UserId, string CategoryId, string ChildCategoryId = "") : ICategoriesQuery<Result<CategoryDto>>;


public class GetCategoryHandler(ICategoriesRepository repository) : IRequestHandler<GetCategoryQuery, Result<CategoryDto>>
{
    private readonly ICategoriesRepository _repository = repository ??
                                                          throw new ArgumentNullException(nameof(repository));

    public async Task<Result<CategoryDto>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var parentCategoryResult = await _repository.GetAsync(request.UserId, request.CategoryId, cancellationToken);

        if (parentCategoryResult.IsFailed)
            return Result.Fail(parentCategoryResult.Errors);

        var parentCategoryDocument = parentCategoryResult.ValueOrDefault;

        if (parentCategoryDocument is null)
            return Result.Fail("Could not find the Category");

        if (string.IsNullOrWhiteSpace(request.ChildCategoryId))
        {
            var parentCategoryDto = Mapper.Map(parentCategoryResult.Value);

            if (parentCategoryDto is null)
                throw new InvalidOperationException(
                    $"Could not map the CategoryDocument '{parentCategoryResult.Value.Id} to its DTO");

            return parentCategoryDto;
        }

        var childCategoryDocument = parentCategoryDocument.SubCategories.ToList().Find(s => s.Id.Value.Equals(request.ChildCategoryId));

        if (childCategoryDocument is null)
            return Result.Fail($"Could not find the Child Category with Id: `{request.ChildCategoryId}`");

        var childCategoryDto = Mapper.Map(childCategoryDocument);

        if (childCategoryDto is null)
            throw new InvalidOperationException(
                $"Could not map the CategoryDocument '{childCategoryDocument.Id} to its DTO");

        return childCategoryDto;
    }
}

public class Validator : AbstractValidator<GetCategoryQuery>
{
    public Validator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
