using System.Globalization;
using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// This is for manual changes to the Credit Limit, which is logged.
/// 
/// This should have its own Api endpoint.
/// </summary>
public class AdjustCreditLimit
{
    public record Command(
        string UserId,
        string AccountId,
        decimal CreditLimit,
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

            // Check if the account supports Credit Limits
            if (existingResult.Value is not IHasCreditLimit)
                return Result.Fail<decimal>($"The Account Type `{existingResult.Value.AccountTypes}` does not support Credit Limits");
            //return Result.Fail<decimal>($"The Account Type `{existingResult.Value.GetType().Name}` does not support Credit Limits");

            var existingAccount = existingResult.Value;

            var existingCreditLimitAccount = existingAccount as IHasCreditLimit;

            var previousCreditLimit = existingCreditLimitAccount.CreditLimit;

            // Updating the existingCreditLimitAccount.CreditLimit will update the existingAccount.CreditLimit.
            // Cannot access the existingAccount.CreditLimit directly because it does not belong to the base AccountDocument.
            existingCreditLimitAccount.CreditLimit = request.CreditLimit;

            existingAccount.AddChangeHistory(
                existingAccount.UserId,
                nameof(IHasCreditLimit.CreditLimit),
                previousCreditLimit.ToString(CultureInfo.InvariantCulture),
                request.CreditLimit.ToString(CultureInfo.InvariantCulture),
                request.Reason);

            var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

            if (result.IsFailed)
                return Result.Fail<decimal>(result.Errors);

            return Result.Ok(request.CreditLimit);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0);
        }
    }
}