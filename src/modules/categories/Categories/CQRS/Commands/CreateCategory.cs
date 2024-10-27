using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Categories.Data;
using Habanerio.Xpnss.Modules.Categories.DTOs;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Categories.CQRS.Commands;

public class CreateCategory
{
    public record Command(
        string UserId,
        string Name,
        string Description = "",
        int SortOrder = 99) : ICategoriesCommand<Result<CategoryDto>>, IRequest
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

            var categoryDoc = CategoryDocument.New(
                request.UserId,
                request.Name,
                request.Description,
                sortOrder: request.SortOrder);

            var result = await _repository.AddAsync(categoryDoc, cancellationToken);

            if (result.IsFailed)
                return Result.Fail(result.Errors[0].Message ?? "Could not save the Category");

            var categoryDto = Mappers.DocumentToDtoMappings.Map(result.Value);

            if (categoryDto is null)
                return Result.Fail("Failed to map CategoryDocument to CategoryDto");

            // If the Category is created, and we can map it to a DTO,
            // we need to update the SortOrder of all Categories
            // This is just in case there are duplicate sort orders or gaps
            var allCategories = (await _repository.ListAsync(request.UserId, cancellationToken))
                .Value
                .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
                .ToList();

            var newSortOrder = 1;
            foreach (var allCategory in allCategories)
            {
                allCategory.SortOrder = newSortOrder;
                await _repository.UpdateAsync(request.UserId, allCategory, cancellationToken);

                newSortOrder++;
            }

            return categoryDto;
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.SortOrder).GreaterThan(0);
            }
        }
    }
}