using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Accounts.Application.Commands.OverdraftAdjustment;

/// <summary>
/// This is for manual changes to the Overdraft TotalAmount, which is logged.
/// </summary>
public sealed record AddOverdraftAdjustmentCommand(
    string UserId,
    string AccountId,
    decimal OverdraftAmount,
    DateTime DateOfChange,
    string Reason = "") : IAccountsCommand<Result<decimal>>, IRequest;

/// <summary>
/// Adds an Overdraft Adjustment to an Account.
/// An Overdraft Adjustment is a manual change to the Account's Overdraft
/// to let the user keep the Account in sync with their real-world account.
/// </summary>
public sealed class AddOverdraftAdjustmentHandler(IAccountsRepository repository)
    : IRequestHandler<AddOverdraftAdjustmentCommand, Result<decimal>>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result<decimal>> Handle(AddOverdraftAdjustmentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();

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
            return Result.Fail($"Account with Id `{request.AccountId}` does not exist for User `{request.UserId}`");

        // Check if the Account supports Credit Limits
        if (existingAccount is not IHasOverdraftAmount existingCreditLimitAccount)
            return Result.Fail($"the Account Type `{existingAccount.AccountType}` does not support Overdrafts");

        //existingCreditLimitAccount.AddOverdraftAdjustment(new Money(request.OverdraftAmount), request.DateOfChange, request.Reason);

        var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(request.OverdraftAmount);
    }

    public class Validator : AbstractValidator<AddOverdraftAdjustmentCommand>
    {
        public Validator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.OverdraftAmount).GreaterThanOrEqualTo(0);
        }
    }
}