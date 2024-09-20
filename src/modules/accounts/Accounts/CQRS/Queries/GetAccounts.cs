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

            var dtos = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (dtos.IsFailed)
                return Result.Fail(dtos.Errors[0].Message);

            return Result.Ok(dtos.Value);
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