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
                return Result.Fail<decimal>(validationResult.Errors[0].ErrorMessage);

            var existingResult =
                await _repository.GetByIdAsync(request.UserId, request.AccountId, cancellationToken);

            if (existingResult.IsFailed)
                return Result.Fail<decimal>(existingResult.Errors);

            // Check if the account supports InterestRates
            if (existingResult.Value is not IHasInterestRate)
                return Result.Fail<decimal>($"The Account Type `{existingResult.Value.AccountTypes}` does not support Interest Rates");

            var existingAccount = existingResult.Value;

            var existingInterestRateAccount = existingAccount as IHasInterestRate;

            var previousInterestRate = existingInterestRateAccount.InterestRate;

            existingInterestRateAccount.InterestRate = request.InterestRate;

            existingAccount.AddChangeHistory(
                existingAccount.UserId,
                nameof(IHasInterestRate.InterestRate),
                previousInterestRate.ToString(CultureInfo.InvariantCulture),
                request.InterestRate.ToString(CultureInfo.InvariantCulture),
                request.Reason);

            var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

            if (result.IsFailed)
                return Result.Fail<decimal>(result.Errors);

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