using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Queries;

/// <summary>
/// MediatR query to get accounts by user id.
/// </summary>
public class GetAccounts
{
    public record Query(string UserId) : IAccountsQuery<Result<IEnumerable<AccountDto>>>;

    public class Handler(IAccountsRepository repository) : IRequestHandler<Query, Result<IEnumerable<AccountDto>>>
    {
        private readonly IAccountsRepository _repository = repository ??
            throw new ArgumentNullException(nameof(repository));

        public async Task<Result<IEnumerable<AccountDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validator = new Validator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            var docsResult = await _repository.ListByUserIdAsync(request.UserId, cancellationToken);

            if (docsResult.IsFailed)
                return Result.Fail(docsResult.Errors);

            if (!docsResult.Value.Any())
                return Result.Ok(Enumerable.Empty<AccountDto>());

            var dtos = Mappers.DocumentToDtoMappings.Map(docsResult.Value);

            return Result.Ok(dtos);
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