using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Categories.Application.Mappers;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Shared.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Categories.Application.Commands;

public sealed record UpdateCategoryCommand(
    string UserId,
    string CategoryId,
    string Name,
    string Description,
    int SortOrder) :
    ICategoriesCommand<Result<CategoryDto>>;

public class UpdateCategoryCommandHandler(
    ICategoriesRepository repository) :
    IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoriesRepository _repository = repository ??
         throw new ArgumentNullException(nameof(repository));

    public async Task<Result<CategoryDto>> Handle(
        UpdateCategoryCommand command,
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

        category.Update(new CategoryName(command.Name), command.Description, command.SortOrder);

        var updatedCategoryResult = await _repository.UpdateAsync(command.UserId, category, cancellationToken);

        if (updatedCategoryResult.IsFailed || updatedCategoryResult.ValueOrDefault is null)
            return Result.Fail(updatedCategoryResult.Errors[0].Message ?? "Could not update the Category");

        var updatedCategory = ApplicationMapper.Map(updatedCategoryResult.ValueOrDefault);

        if (updatedCategory is null)
            throw new InvalidOperationException("Failed to map the updated CategoryDocument to CategoryDto");

        return updatedCategory;
    }

    public class Validator : AbstractValidator<UpdateCategoryCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        }
    }
}