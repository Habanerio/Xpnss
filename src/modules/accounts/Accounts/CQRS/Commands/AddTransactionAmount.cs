using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// This is for regular updates to the balance based on transactions.
/// 
/// If user makes manual changes, then use AdjustBalance instead.
/// 
/// This probably won't have its own api endpoint,
/// and instead would be called from the transaction endpoint,
/// or during the transaction process.
/// </summary>
public class AddTransactionAmount
{
    public record Command(
        string UserId,
        string AccountId,
        decimal Amount,
        bool IsCredit,
        DateTime DateOfTransaction,
        string Reason = "") : IAccountsCommand<Result>, IRequest;

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly IAccountsRepository _repository;

        public Handler(IAccountsRepository repository)
        {
            _repository = repository ??
                          throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
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

            if (existingAccount == null)
                return Result.Fail($"No account found for '{request.AccountId}'");

            if (request.IsCredit)
                existingAccount.Deposit(request.DateOfTransaction.ToUniversalTime(), request.Amount);
            else
                existingAccount.Withdraw(request.DateOfTransaction.ToUniversalTime(), request.Amount);

            //switch(existingAccount.AccountType)
            //{
            //    case AccountTypes.CreditCard:
            //        if (request.IsCredit)
            //            existingAccount.Balance -= request.Amount;
            //        else
            //            existingAccount.Balance += request.Amount;
            //        break;
            //    case AccountTypes.Loan:
            //        if (request.IsCredit)
            //            existingAccount.Balance -= request.Amount;
            //        else
            //            existingAccount.Balance += request.Amount;
            //        break;
            //    case AccountTypes.Savings:
            //        if (request.IsCredit)
            //            existingAccount.Balance += request.Amount;
            //        else
            //            existingAccount.Balance -= request.Amount;
            //        break;
            //    case AccountTypes.Checking:
            //        if (request.IsCredit)
            //            existingAccount.Balance += request.Amount;
            //        else
            //            existingAccount.Balance -= request.Amount;
            //        break;
            //    default:
            //        return Result.Fail($"Unknown account type '{existingAccount.AccountType}'");
            //}

            var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

            if (result.IsFailed)
                return Result.Fail(result.Errors);

            return Result.Ok();
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