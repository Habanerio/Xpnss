using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// MediatR Command to Create a new Account
/// </summary>
/// <remarks>
/// Needs to be cleaned up
/// </remarks>
public class CreateAccount
{
    public record Command(
        string UserId,
        AccountType AccountType,
        string Name,
        string Description,
        decimal Balance,
        decimal CreditLimit = 0,
        decimal InterestRate = 0,
        decimal OverDraftAmount = 0,
        string DisplayColor = "") : IAccountsCommand<Result<string>>, IRequest;

    public class Handler(IAccountsRepository repository) : IRequestHandler<Command, Result<string>>
    {
        private readonly IAccountsRepository _repository = repository ??
                                                           throw new ArgumentNullException(nameof(repository));

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validator = new Validator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            var account = GetAccount(request);

            var result = await _repository.AddAsync(account, cancellationToken);

            if (!result.IsSuccess)
                return Result.Fail(result.Errors?[0].Message ?? "Could not save the Account");

            return Result.Ok(result.Value.ToString());
        }

        private AccountDocument GetAccount(Command request)
        {
            return request.AccountType switch
            {
                AccountType.Cash => CashAccount.New(
                    request.UserId,
                    request.Name,
                    request.Description,
                    request.Balance,
                    request.DisplayColor),
                AccountType.Checking => CheckingAccount.New(
                    request.UserId,
                    request.Name,
                    request.Description,
                    request.Balance,
                    request.OverDraftAmount,
                    request.DisplayColor),
                AccountType.Savings => SavingsAccount.New(
                    request.UserId,
                    request.Name,
                    request.Description,
                    request.Balance,
                    request.InterestRate,
                    request.DisplayColor),
                AccountType.CreditCard => CreditCardAccount.New(
                    request.UserId,
                    request.Name,
                    request.Description,
                    request.Balance,
                    request.CreditLimit,
                    request.InterestRate,
                    request.DisplayColor),
                AccountType.LineOfCredit => LineOfCreditAccount.New(
                    request.UserId,
                    request.Name,
                    request.Description,
                    request.Balance,
                    request.CreditLimit,
                    request.InterestRate,
                    request.DisplayColor),
                _ => throw new InvalidOperationException("Account Type not supported")
            };
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountType).IsInEnum();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}