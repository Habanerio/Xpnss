using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Application.Mappers;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Categories.Application.Commands;

/// <summary>
/// Adds a collection of one or more Sub Categories to an existing Category
/// </summary>
public sealed class AddSubCategories
{
    public record Command(
        string UserId,
        string ParentCategoryId,
        IEnumerable<CategoryDto> SubCategories) :
        ICategoriesCommand<Result<CategoryDto>>, IRequest
    { }

    public class Handler(ICategoriesRepository repository) : IRequestHandler<Command, Result<CategoryDto>>
    {
        private readonly ICategoriesRepository _repository = repository ??
              throw new ArgumentNullException(nameof(repository));

        public async Task<Result<CategoryDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validator = new Validator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            var parentResult = await _repository.GetAsync(request.UserId, request.ParentCategoryId, cancellationToken);

            if (parentResult.IsFailed || parentResult.ValueOrDefault is null)
                return Result.Fail(parentResult.Errors[0].Message ?? "Could not find the Parent Category");

            var parentDoc = parentResult.ValueOrDefault;

            var subCategoryDtos = request.SubCategories.ToArray();

            foreach (var subCategoryDto in subCategoryDtos.OrderBy(s => s.SortOrder).ThenBy(s => s.Name))
            {
                var subCategoryValidator = new SubCategoryValidator();

                var subCategoryValidationResult = await subCategoryValidator.ValidateAsync(subCategoryDto, cancellationToken);

                if (!subCategoryValidationResult.IsValid)
                    return Result.Fail(subCategoryValidationResult.Errors[0].ErrorMessage);

                parentDoc.AddSubCategory(new CategoryName(subCategoryDto.Name), subCategoryDto.Description, subCategoryDto.SortOrder);
            }

            var result = await _repository.UpdateAsync(request.UserId, parentDoc, cancellationToken);

            if (result.IsFailed)
                return Result.Fail(result.Errors[0].Message ?? "Could not save the Category");

            var parentCategoryDto = ApplicationMapper.Map(parentDoc);

            if (parentCategoryDto is null)
                return Result.Fail<CategoryDto>("Failed to map CategoryDocument to CategoryDto");

            return parentCategoryDto;
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.ParentCategoryId).NotEmpty();
                RuleFor(x => x.SubCategories).NotEmpty();
            }
        }

        public class SubCategoryValidator : AbstractValidator<CategoryDto>
        {
            public SubCategoryValidator()
            {
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.SortOrder).GreaterThan(0);
            }
        }
    }
}