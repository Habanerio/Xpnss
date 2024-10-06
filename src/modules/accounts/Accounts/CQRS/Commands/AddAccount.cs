using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// MediatR Command to AddDocument an Existing Account (?)
/// </summary>
/// <remarks>
/// Needs to be cleaned up
/// </remarks>
public class AddAccount
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

            AccountDto? accountDto;

            switch (request.AccountType)
            {
                case AccountType.Cash:
                    accountDto = CashAccountDto.New(
                        request.UserId,
                        request.Name,
                        request.Description,
                        request.Balance,
                        request.DisplayColor);
                    break;
                case AccountType.Checking:
                    accountDto = CheckingAccountDto.New(
                        request.UserId,
                        request.Name,
                        request.Description,
                        request.Balance,
                        request.OverDraftAmount,
                        request.DisplayColor);
                    break;
                case AccountType.Savings:
                    accountDto = SavingsAccountDto.New(
                        request.UserId,
                        request.Name,
                        request.Description,
                        request.Balance,
                        request.InterestRate,
                        request.DisplayColor);
                    break;
                case AccountType.CreditCard:
                    accountDto = CreditCardAccountDto.New(
                        request.UserId,
                        request.Name,
                        request.Description,
                        request.Balance,
                        request.CreditLimit,
                        request.InterestRate,
                        request.DisplayColor);
                    break;

                case AccountType.LineOfCredit:
                    accountDto = LineOfCreditAccountDto.New(
                        request.UserId,
                        request.Name,
                        request.Description,
                        request.Balance,
                        request.CreditLimit,
                        request.InterestRate,
                        request.DisplayColor);
                    break;
                default:
                    accountDto = null;
                    break;
            }

            if (accountDto is null)
                return Result.Fail("Account Type not supported");

            var result = _repository.Add(accountDto);

            if (!result.IsSuccess)
                return Result.Fail(result.Errors?[0].Message ?? "Could not save the Account");

            return Result.Ok(result.Value.ToString());
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