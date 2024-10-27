using System.Globalization;
using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// This is for manual changes to the Interest Rate, which is logged.
/// 
/// This should have its own Api endpoint.
/// </summary>
public class AdjustInterestRate
{
    public record Command(
        string UserId,
        string AccountId,
        decimal InterestRate,
        DateTime DateOfChange,
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

            // Check if the account supports InterestRates
            if (existingResult.Value is not IHasInterestRate)
                return Result.Fail($"The Account Type `{existingResult.Value.AccountType}` does not support Interest Rates");

            var existingAccount = existingResult.Value;

            if (!(existingAccount is IHasInterestRate interestRateAccount))
                return Result.Fail("Account does not have an interest rate");

            var previousInterestRate = interestRateAccount.InterestRate;

            interestRateAccount.InterestRate = request.InterestRate;

            existingAccount.AddChangeHistory(
                existingAccount.UserId,
                nameof(IHasInterestRate.InterestRate),
                previousInterestRate.ToString(CultureInfo.InvariantCulture),
                request.InterestRate.ToString(CultureInfo.InvariantCulture),
                request.DateOfChange.ToUniversalTime(),
                request.Reason);

            var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

            if (result.IsFailed)
                return Result.Fail(result.Errors);

            return Result.Ok(request.InterestRate);
        }
    }

    public class Validator : AbstractValidator<Command>
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