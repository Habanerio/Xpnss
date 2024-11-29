using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Application.Mappers;
using Habanerio.Xpnss.Categories.Domain.Entities;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Categories.Application.Commands;

public sealed record CreateCategoryCommand(
    string UserId,
    string Name,
    string? ParentId = null,
    string Description = "",
    int SortOrder = 99) :
    ICategoriesCommand<Result<CategoryDto>>;

public class CreateCategoryCommandHandler(ICategoriesRepository repository) :
    IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoriesRepository _repository = repository ??
         throw new ArgumentNullException(nameof(repository));

    public async Task<Result<CategoryDto>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        if (!string.IsNullOrWhiteSpace(request.ParentId) && !request.ParentId.Equals(ObjectId.Empty.ToString()))
        {
            var parentResult = await _repository.GetAsync(request.UserId, request.ParentId, cancellationToken);

            if (parentResult.IsFailed || parentResult.ValueOrDefault is null)
                return Result.Fail(parentResult.Errors[0].Message ?? "Could not find the Parent Category");

            var parentDoc = parentResult.ValueOrDefault;

            var subCategory = parentDoc.AddSubCategory(new CategoryName(request.Name), request.Description, request.SortOrder);

            var parentUpdateResult = await _repository.UpdateAsync(request.UserId, parentDoc, cancellationToken);

            if (parentUpdateResult.IsFailed)
                return Result.Fail(parentUpdateResult.Errors[0].Message ?? "Could not save the Parent Category");

            var childDto = ApplicationMapper.Map(subCategory);

            if (childDto is null)
                return Result.Fail("Failed to map Sub CategoryDocument to SubCategoryDto");

            return childDto;
        }

        var category = Category.New(
            new UserId(request.UserId),
            new CategoryName(request.Name),
            request.Description,
            CategoryId.Empty,
            request.SortOrder);

        var result = await _repository.AddAsync(category, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors[0].Message ?? "Could not save the Category");

        var allCategoriesResult = await _repository.ListAsync(request.UserId, cancellationToken);

        if (!allCategoriesResult.Value.Any())
            return Result.Fail("No Categories found for the User");

        var allCategories = allCategoriesResult.Value.ToList();

        // Make sure that all Categories for this user are in order with no gaps
        var newSortOrder = 1;
        foreach (var allCategory in allCategories)
        {
            allCategory.SortOrder = newSortOrder;
            await _repository.UpdateAsync(request.UserId, allCategory, cancellationToken);

            newSortOrder++;
        }

        // Get the Category that was just updated
        var categoryDto = ApplicationMapper.Map(allCategories.Find(c => c.Id.Equals(result.Value.Id)));

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
            RuleFor(x => x.SortOrder).GreaterThan(0);
        }
    }
}