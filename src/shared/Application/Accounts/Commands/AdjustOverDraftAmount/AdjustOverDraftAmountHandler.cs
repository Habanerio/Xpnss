using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Application.Accounts.Commands.AdjustOverDraftAmount;

/// <summary>
/// This is for manual changes to the Overdraft Amount, which is logged.
/// 
/// This should have its own Api endpoint.
/// </summary>
public sealed class AdjustOverDraftAmountHandler(IAccountsRepository repository)
    : IRequestHandler<AdjustOverDraftAmountCommand, Result<decimal>>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result<decimal>> Handle(AdjustOverDraftAmountCommand request, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var existingResult =
            await _repository.GetAsync(request.UserId, request.AccountId, cancellationToken);

        if (existingResult.IsFailed)
            return Result.Fail(existingResult.Errors);

        // Check if the account supports Credit Limits
        if (existingResult.Value is not IHasCreditLimit)
            return Result.Fail($"The Account Type `{existingResult.Value.AccountType}` does not support Over Drafts");

        var existingAccount = existingResult.Value;

        var existingCreditLimitAccount = existingAccount as IHasOverDraftAmount;

        if (existingCreditLimitAccount is null)
            return Result.Fail($"The Account Type `{existingAccount.AccountType}` does not support Over Drafts");

        existingCreditLimitAccount.AdjustOverDraftAmount(new Money(request.OverDraftAmount), request.DateOfChange, request.Reason);

        var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(request.OverDraftAmount);
    }

    public class Validator : AbstractValidator<AdjustOverDraftAmountCommand>
    {
        public Validator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.OverDraftAmount).GreaterThanOrEqualTo(0);
        }
    }
}