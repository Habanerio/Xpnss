using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Application.Accounts.Commands.AddTransactionAmount;

/// <summary>
/// This is for regular updates to the balance based on transactions.
/// 
/// If user makes manual changes, then use AdjustBalance instead.
/// 
/// This probably won't have its own api endpoint,
/// and instead would be called from the transaction endpoint,
/// or during the transaction process.
/// </summary>

public sealed class AddTransactionAmountHandler(IAccountsRepository repository)
    : IRequestHandler<AddTransactionAmountCommand, Result>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result> Handle(AddTransactionAmountCommand request, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var existingResult =
            await _repository.GetAsync(request.UserId, request.AccountId, cancellationToken);

        if (existingResult.IsFailed)
            return Result.Fail(existingResult.Errors);

        var existingAccount = existingResult.Value;

        if (existingAccount == null)
            return Result.Fail($"No account found for '{request.AccountId}'");

        if (request.IsCredit)
            existingAccount.Deposit(request.DateOfTransaction.ToUniversalTime(), new Money(request.Amount));
        else
            existingAccount.Withdraw(request.DateOfTransaction.ToUniversalTime(), new Money(request.Amount));

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

    internal sealed class Validator : AbstractValidator<AddTransactionAmountCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }
}