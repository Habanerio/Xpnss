using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Accounts.Application.Commands.CreditLimitAdjustment;

/// <summary>
/// Adds a Credit Limit Adjustment to an Account.
/// A Credit Limit Adjustment is a manual change to the Account's Credit Limit
/// to let the user keep the Account in sync with their real-world account.
/// </summary>
public sealed record AddCreditLimitAdjustmentCommand(
    string UserId,
    string AccountId,
    decimal CreditLimit,
    DateTime DateOfChange,
    string Reason = "") : IAccountsCommand<Result<decimal>>, IRequest;

/// <summary>
/// This is for manual changes to the Credit Limit, which is logged.
/// 
/// This should have its own Api endpoint.
/// </summary>
public sealed class AddCreditLimitAdjustmentHandler(IAccountsRepository repository)
    : IRequestHandler<AddCreditLimitAdjustmentCommand, Result<decimal>>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result<decimal>> Handle(AddCreditLimitAdjustmentCommand request, CancellationToken cancellationToken)
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

        // Check if the Account supports Credit Limits
        if (existingResult.Value is not IHasCreditLimit)
            return Result.Fail($"the Account Type `{existingResult.Value.AccountType}` does not support Credit Limits");

        var existingAccount = existingResult.Value;

        var existingCreditLimitAccount = existingAccount as IHasCreditLimit;

        if (existingCreditLimitAccount is null)
            return Result.Fail($"the Account Type `{existingAccount.AccountType}` does not support Credit Limits");

        //existingCreditLimitAccount.AddCreditLimitAdjustment(new Money(request.CreditLimit), request.DateOfChange, request.Reason);

        var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(request.CreditLimit);
    }

    public class Validator : AbstractValidator<AddCreditLimitAdjustmentCommand>
    {
        public Validator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0);
        }
    }
}