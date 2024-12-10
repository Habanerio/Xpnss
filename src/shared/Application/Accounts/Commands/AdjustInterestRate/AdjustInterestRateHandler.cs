using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Application.Accounts.Commands.AdjustInterestRate;

/// <summary>
/// This is for manual changes to the Interest Rate, which is logged.
/// </summary>
public sealed class AdjustInterestRateHandler(IAccountsRepository repository)
    : IRequestHandler<AdjustInterestRateCommand, Result<decimal>>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result<decimal>> Handle(AdjustInterestRateCommand request, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var existingResult =
            await _repository.GetAsync(request.UserId, request.AccountId, cancellationToken);

        if (existingResult.IsFailed)
            return Result.Fail(existingResult.Errors);

        // Check if the account supports InterestRates
        if (existingResult.Value is not IHasInterestRate)
            return Result.Fail($"The Account Type `{existingResult.Value.AccountType}` does not support Interest Rates");

        var existingAccount = existingResult.Value;

        if (!(existingAccount is IHasInterestRate interestRateAccount))
            return Result.Fail("Account does not have an interest rate");

        interestRateAccount.AdjustInterestRate(new PercentageRate(request.InterestRate), request.DateOfChange, request.Reason);

        var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(request.InterestRate);
    }

    internal sealed class Validator : AbstractValidator<AdjustInterestRateCommand>
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


