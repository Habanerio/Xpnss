using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Accounts.Application.Commands.InterestRateAdjustment;

public sealed record AddInterestRateAdjustmentCommand(
    string UserId,
    string AccountId,
    decimal InterestRate,
    DateTime DateOfChange,
    string Reason = "") : IAccountsCommand<Result<decimal>>, IRequest;

/// <summary>
/// Adds an Interest Rate Adjustment to an Account.
/// An Interest Rate Adjustment is a manual change to the Account's Interest Rate
/// to let the user keep the Account in sync with their real-world account.
/// </summary>
public sealed class AddInterestRateAdjustmentHandler(IAccountsRepository repository)
    : IRequestHandler<AddInterestRateAdjustmentCommand, Result<decimal>>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result<decimal>> Handle(AddInterestRateAdjustmentCommand request, CancellationToken cancellationToken)
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

        if (existingResult.ValueOrDefault is null)
            return Result.Fail($"Account with Id `{request.AccountId}` does not exist for User `{request.UserId}`");

        var existingAccount = existingResult.Value;

        // Check if the Account supports InterestRates
        if (existingAccount is not IHasInterestRate interestRateAccount)
            return Result.Fail($"the Account Type `{existingAccount!.AccountType}` does not support Interest Rates");

        //interestRateAccount.AddInterestRateAdjustment(new PercentageRate(request.InterestRate), request.DateOfChange, request.Reason);

        var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(request.InterestRate);
    }

    internal sealed class Validator : AbstractValidator<AddInterestRateAdjustmentCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.InterestRate)
                .InclusiveBetween(0, 100);
        }
    }
}