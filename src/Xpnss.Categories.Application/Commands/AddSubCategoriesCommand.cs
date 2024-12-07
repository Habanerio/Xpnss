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
public record AddSubCategoriesCommand(
    string UserId,
    string ParentCategoryId,
    IEnumerable<SubCategoryDto> SubCategories) :
    ICategoriesCommand<Result<CategoryDto>>, IRequest
{ }

public class AddSubCategoriesCommandHandler(ICategoriesRepository repository) :
    IRequestHandler<AddSubCategoriesCommand, Result<CategoryDto>>
{
    private readonly ICategoriesRepository _repository = repository ??
          throw new ArgumentNullException(nameof(repository));

    public async Task<Result<CategoryDto>> Handle(
        AddSubCategoriesCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var parentCatResult = await _repository.GetAsync(
            request.UserId, request.ParentCategoryId, cancellationToken);

        if (parentCatResult.IsFailed || parentCatResult.ValueOrDefault is null)
            return Result.Fail(parentCatResult.Errors[0].Message ?? "Could not find the Parent Category");

        var parentCatDoc = parentCatResult.ValueOrDefault;

        var subCatDtos = request.SubCategories.ToArray();

        for (var i = 0; i < subCatDtos.Length; i++)
        {
            var subCatDto = subCatDtos[i];

            var subCatValidator = new SubCategoryValidator();

            var subCategoryValidationResult = await subCatValidator.ValidateAsync(subCatDto, cancellationToken);

            if (!subCategoryValidationResult.IsValid)
                return Result.Fail($"SubCategory at index {i} had errors: {subCategoryValidationResult.Errors[0].ErrorMessage}");

            parentCatDoc.AddSubCategory(
                new CategoryName(subCatDto.Name),
                subCatDto.Description,
                subCatDto.SortOrder);
        }

        var result = await _repository.UpdateAsync(request.UserId, parentCatDoc, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors[0].Message ?? "Could not save the Category");

        var parentCategoryDto = ApplicationMapper.Map(parentCatDoc);

        if (parentCategoryDto is null)
            return Result.Fail<CategoryDto>("Failed to map CategoryDocument to CategoryDto");

        return parentCategoryDto;
    }

    public class Validator : AbstractValidator<AddSubCategoriesCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ParentCategoryId).NotEmpty();
            RuleFor(x => x.SubCategories).NotEmpty();
        }
    }

    public class SubCategoryValidator : AbstractValidator<SubCategoryDto>
    {
        public SubCategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.SortOrder).GreaterThan(0);
        }
    }
}