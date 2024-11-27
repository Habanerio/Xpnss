using System.Text.Json.Serialization;
using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Accounts.Application.Commands.BalanceAdjustment;

/// <summary>
/// This is for manual changes to the Account Balance, which is logged.
/// 
/// Do not use this for regular updates to the balance based on transactions.
/// 
/// This should have its own Api endpoint.
/// </summary>
public sealed record AddBalanceAdjustmentCommand : IAccountsCommand<Result<decimal>>
{
    public string UserId { get; set; } = "";

    public string AccountId { get; set; } = "";

    public decimal Balance { get; set; } = 0;

    public DateTime DateOfChange { get; set; }

    public string Reason { get; set; } = "";

    [JsonConstructor]
    public AddBalanceAdjustmentCommand() { }

    public AddBalanceAdjustmentCommand(string userId, string accountId, decimal balance, DateTime dateOfChange, string reason = "")
    {
        UserId = userId;
        AccountId = accountId;
        Balance = balance;
        DateOfChange = dateOfChange;
        Reason = reason;
    }
}

/// <summary>
/// Adds a Balance Adjustment to an Account.
/// A Balance Adjustment is a manual change to the Account's Balance
/// to let the user keep the Account in sync with their real-world account.
/// </summary>
/*
    TODO: Need to fire off an event to update the Account's Balance from the time of the Adjustment,
    to the next Adjustment (if one exists) or until today.
    MonthlyTotals will need to be updated as well.
*/
public sealed class AddBalanceAdjustmentHandler(IAccountsRepository repository)
    : IRequestHandler<AddBalanceAdjustmentCommand, Result<decimal>>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result<decimal>> Handle(AddBalanceAdjustmentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException(
            "Need to rethink this. Some new service may need to be introduced to recalculate and update the balance");


        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var existingResult =
            await _repository.GetAsync(request.UserId, request.AccountId, cancellationToken);

        if (existingResult.IsFailed)
            return Result.Fail(existingResult.Errors);

        var existingAccount = existingResult.ValueOrDefault;

        if (existingAccount is null)
            return Result.Fail($"Account '{request.AccountId}' could not be found for user '{request.UserId}'");

        //existingAccount.AddBalanceAdjustment(new Money(request.Balance), request.DateOfChange, request.Reason);

        var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(request.Balance);
    }

    internal class Validator : AbstractValidator<AddBalanceAdjustmentCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }
}