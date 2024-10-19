using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// This is for regular updates to the balance based on transactions.
/// 
/// If user makes manual changes, then use AdjustBalance instead.
/// 
/// This probably won't have its own api endpoint,
/// and instead would be called from the transaction endpoint,
/// or during the transaction process.
/// </summary>
public class UpdateCreditLimit
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
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            var existingResult =
                await _repository.GetByIdAsync(request.UserId, request.AccountId, cancellationToken);

            if (existingResult.IsFailed)
                return Result.Fail(existingResult.Errors);

            var existingAccount = existingResult.Value;

            if (!(existingAccount is IHasCreditLimit creditLimitAccount))
                return Result.Fail($"The Account Type `{existingAccount.AccountType}` does not support Credit Limits");

            var previousCreditLimit = creditLimitAccount.CreditLimit;

            creditLimitAccount.CreditLimit = request.CreditLimit;

            // No AddChangeHistory for Updates (only Adjustments)

            var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

            if (result.IsFailed)
                return Result.Fail(result.Errors);

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