using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Application.Mappers;
using Habanerio.Xpnss.Categories.Domain.Entities;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Categories.Application.Commands;

public sealed record CreateCategoryCommand(
    string UserId,
    string Name,
    string Description,
    IEnumerable<SubCategoryDto>? SubCategories = null) :
    ICategoriesCommand<Result<CategoryDto>>;

public class CreateCategoryCommandHandler(
    ICategoriesRepository repository) :
    IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoriesRepository _repository = repository ??
         throw new ArgumentNullException(nameof(repository));

    public async Task<Result<CategoryDto>> Handle(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        // Check if it exists, and if so, return the existing Category
        var doesCategoryExistResult = await _repository.ExistsAsync(
            new UserId(command.UserId),
            new CategoryName(command.Name),
            cancellationToken);

        if (doesCategoryExistResult.Value)
        {
            var existingCategory = await _repository.GetAsync(
                new UserId(command.UserId),
                new CategoryName(command.Name),
                cancellationToken);

            var existingCategoryDto = ApplicationMapper.Map(existingCategory.ValueOrDefault);

            if (existingCategoryDto is null)
                return Result.Fail($"Category '{command.Name}' already exists, " +
                                   $"but failed to map the existing CategoryDocument to CategoryDto");

            return existingCategoryDto;
        }

        var category = Category.New(
            new UserId(command.UserId),
            new CategoryName(command.Name),
            command.Description, 99);

        var result = await _repository.AddAsync(category, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors[0].Message ?? "Could not save the Category");

        var subCategoryDtos = command.SubCategories?.ToArray() ?? [];

        if (subCategoryDtos.Any())
        {
            var sortOrder = 1;
            foreach (var subCategoryDto in subCategoryDtos)
            {
                category.AddSubCategory(
                    new CategoryName(subCategoryDto.Name), category.Description, sortOrder);

                var updateResult = await _repository.UpdateAsync(command.UserId, category, cancellationToken);

                sortOrder++;
            }
        }

        var allCategoriesResult = await _repository.ListAsync(command.UserId, cancellationToken);

        if (!allCategoriesResult.Value.Any())
            return Result.Fail("No Categories found for the User");

        var allCategories = allCategoriesResult.Value.ToList();

        // Make sure that all Categories for this user are in order with no gaps
        var newSortOrder = 1;
        foreach (var allCategory in allCategories)
        {
            allCategory.SortOrder = newSortOrder;
            await _repository.UpdateAsync(command.UserId, allCategory, cancellationToken);

            newSortOrder++;
        }

        // Get the Category that was just updated
        var categoryDto = ApplicationMapper.Map(allCategories.Find(c =>
            c.Id.Equals(result.Value.Id)));

        if (categoryDto is null)
            return Result.Fail("Failed to map CategoryDocument to CategoryDto");

        return categoryDto;
    }

    public class Validator : AbstractValidator<CreateCategoryCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}