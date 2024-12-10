using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Categories.Application.Mappers;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Shared.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Categories.Application.Commands;

public sealed record UpdateSubCategoryCommand(
    string UserId,
    string CategoryId,
    string SubCategoryId,
    string Name,
    string Description,
    int SortOrder) :
    ICategoriesCommand<Result<SubCategoryDto>>;

public class UpdateSubCategoryCommandHandler(
    ICategoriesRepository repository) :
    IRequestHandler<UpdateSubCategoryCommand, Result<SubCategoryDto>>
{
    private readonly ICategoriesRepository _repository = repository ??
         throw new ArgumentNullException(nameof(repository));

    public async Task<Result<SubCategoryDto>> Handle(
        UpdateSubCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var categoryResult = await _repository.GetAsync(
            new UserId(command.UserId),
            new CategoryId(command.CategoryId),
            cancellationToken);

        if (categoryResult.IsFailed)
            return Result.Fail(categoryResult.Errors[0].Message);

        if (categoryResult.ValueOrDefault is null)
            return Result.Fail($"Category '{command.CategoryId}' not found");

        var category = categoryResult.ValueOrDefault;

        category.UpdateSubCategory(command.SubCategoryId, command.Name, command.Description, command.SortOrder);

        var updatedCategoryResult = await _repository.UpdateAsync(command.UserId, category, cancellationToken);

        if (updatedCategoryResult.IsFailed || updatedCategoryResult.ValueOrDefault is null)
            return Result.Fail(updatedCategoryResult.Errors[0].Message ?? "Could not update the Category");

        var updatedCategory = ApplicationMapper.Map(updatedCategoryResult.ValueOrDefault);

        if (updatedCategory is null)
            throw new InvalidOperationException("Failed to map the updated CategoryDocument to CategoryDto");

        var subCategory = updatedCategory.SubCategories.Find(x => x.Id == command.SubCategoryId);

        if (subCategory is null)
            return Result.Fail($"Houston, we have a problem. " +
                               $"SubCategory '{command.SubCategoryId}' could not be found after updating");

        return Result.Ok(subCategory);
    }

    public class Validator : AbstractValidator<UpdateSubCategoryCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.SubCategoryId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        }
    }
}