using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Categories.DTOs;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Categories.CQRS.Queries;

public class GetCategories
{
    public record Query(string UserId) : IRequest<Result<IEnumerable<CategoryDto>>> { }

    public class Handler(ICategoriesRepository repository) : IRequestHandler<Query, Result<IEnumerable<CategoryDto>>>
    {
        private readonly ICategoriesRepository _repository = repository ??
                                                             throw new ArgumentNullException(nameof(repository));

        public async Task<Result<IEnumerable<CategoryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validator = new Validator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            try
            {
                var docsResult = await _repository.ListAsync(request.UserId, cancellationToken);

                if (docsResult.IsFailed)
                    return Result.Fail(docsResult.Errors);

                if (!docsResult.Value.Any())
                    return Result.Ok(Enumerable.Empty<CategoryDto>());

                var dtos = Mappers.DocumentToDtoMappings.Map(docsResult.Value);

                return Result.Ok(dtos);
            }
            catch (Exception e)
            {
                return Result.Fail(e.Message);
            }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
            }
        }
    }
}