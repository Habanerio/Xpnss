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

            return categoryDto;
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.Name).NotEmpty();
            }
        }
    }
}