using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Application.Mappers;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Accounts.Application.Commands.CreateAccount;

public sealed class CreateAccountCommandHandler(IAccountsRepository repository) : IRequestHandler<CreateAccountCommand, Result<AccountDto>>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result<AccountDto>> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var account = GetAccountFromCommand(command);

        var result = await _repository.AddAsync(account, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ?? "Could not save the Account");

        var accountDto = ApplicationMapper.Map(result.ValueOrDefault);

        if (accountDto is null)
            return Result.Fail("Failed to map AccountDocument to AccountDto");

        return Result.Ok(accountDto);
    }

    /// <summary>
    /// Creates an AccountDocument from the CreateAccountCommand
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static BaseAccount GetAccountFromCommand(CreateAccountCommand command)
    {
        return command.AccountType switch
        {
            nameof(AccountTypes.Keys.Cash) => CashAccount.New(
                new UserId(command.UserId),
                new AccountName(command.Name),
                command.Description,
                command.DisplayColor),

            nameof(AccountTypes.Keys.Checking) => CheckingAccount.New(
                new UserId(command.UserId),
                new AccountName(command.Name),
                command.Description,
                command.DisplayColor,
                new Money(command.OverdraftAmount)),

            nameof(AccountTypes.Keys.Savings) => SavingsAccount.New(
                new UserId(command.UserId),
                new AccountName(command.Name),
                command.Description,
                command.DisplayColor,
                new PercentageRate(command.InterestRate)),

            nameof(AccountTypes.Keys.CreditCard) => CreditCardAccount.New(
                new UserId(command.UserId),
                new AccountName(command.Name),
                command.Description,
                command.DisplayColor,
                new Money(command.CreditLimit),
                new PercentageRate(command.InterestRate)),

            nameof(AccountTypes.Keys.LineOfCredit) => LineOfCreditAccount.New(
                new UserId(command.UserId),
                new AccountName(command.Name),
                command.Description,
                command.DisplayColor,
                new Money(command.CreditLimit),
                new PercentageRate(command.InterestRate)),

            _ => throw new InvalidOperationException("Account Type not supported")
        };
    }

    internal sealed class Validator : AbstractValidator<CreateAccountCommand>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            //RuleFor(x => x.AccountType).IsInEnum();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}

