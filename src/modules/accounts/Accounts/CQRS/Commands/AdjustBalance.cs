using System.Globalization;
using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// This is for manual changes to the Account Balance, which is logged.
/// 
/// Do not use this for regular updates to the balance based on transactions.
/// 
/// This should have its own Api endpoint.
/// </summary>
public class AdjustBalance
{
    public record Command(
        string UserId,
        string AccountId,
        decimal Balance,
        string Reason = "") : IAccountsCommand<Result<decimal>>, IRequest;

    public class Handler : IRequestHandler<Command, Result<decimal>>
    {
        private readonly IAccountsRepository _repository;

        public Handler(IAccountsRepository repository)
        {
            _repository = repository ??
                          throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<decimal>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validator = new Validator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            var existingResult =
                await _repository.GetByIdAsync(request.UserId, request.AccountId, cancellationToken);

            if (existingResult.IsFailed)
                return Result.Fail(existingResult.Errors);

            var existingAccount = existingResult.Value;

            var previousBalance = existingAccount.Balance;

            existingAccount.Balance = request.Balance;

            // This is causing the error
            existingAccount.AddChangeHistory(
                existingAccount.UserId,
                nameof(existingAccount.Balance),
                previousBalance.ToString(CultureInfo.InvariantCulture),
                existingAccount.Balance.ToString(CultureInfo.InvariantCulture),
                request.Reason);

            var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

            if (result.IsFailed)
                return Result.Fail(result.Errors);

            return Result.Ok(request.Balance);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }
}