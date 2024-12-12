using FluentResults;

using FluentValidation;
using Habanerio.Xpnss.Accounts.Application.Commands.UpdateAccountDetails;
using Habanerio.Xpnss.Accounts.Application.Mappers;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CashAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CreditCardAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.LoanAccounts;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Accounts.Application.Commands.CreateAccount;

/// <summary>
/// MediatR Command to Create a new Account
/// </summary>
/// <remarks>
/// Needs to be cleaned up
/// </remarks>
public record CreateAccountCommand(
    string UserId,
    CreateAccountApiRequest Request) :
    IAccountsCommand<Result<AccountDto>>;

public sealed class CreateAccountCommandHandler(IAccountsRepository repository, IMediator mediator) :
    IRequestHandler<CreateAccountCommand, Result<AccountDto>>
{
    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    private readonly IAccountsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<AccountDto>> Handle(
        CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var account = GetAccountFromRequest(command.UserId, command.Request);

        var result = await _repository.AddAsync(account, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ?? "Could not save the Account");

        var accountDto = ApplicationMapper.Map(result.ValueOrDefault);

        if (accountDto is null)
            throw new InvalidCastException("Failed to map AccountDocument to AccountDto");

        // Undefault any/all other accounts
        if (account.IsDefault)
        {
            var allAccounts =
                (await _repository.ListAsync(command.UserId, cancellationToken)).ValueOrDefault?.ToList() ?? [];

            var otherDefaultAccounts = allAccounts.Where(a =>
                a.Id != account.Id.Value)
                .OrderBy(a => a.SortOrder)
                .ThenBy(a => a.Name.Value)
                .ToList();

            // Start at 2 because the first Account is already set to 1
            var sortOrder = 2;

            foreach (var otherDefaultAccount in otherDefaultAccounts)
            {
                var updateDetailsCommand = new UpdateAccountDetailsCommand(
                    command.Request.UserId,
                    new UpdateAccountDetailsApiRequest
                    {
                        UserId = command.UserId,
                        AccountId = otherDefaultAccount.Id,
                        IsDefault = false,
                        SortOrder = sortOrder
                    });

                sortOrder++;

                await _mediator.Send(updateDetailsCommand, cancellationToken);
            }
        }

        return Result.Ok(accountDto);
    }

    /// <summary>
    /// Creates an AccountDocument from the CreateAccountCommand
    /// </summary>
    /// <param name="userId">The id of the user from the command</param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static AbstractAccountBase GetAccountFromRequest(string userId, CreateAccountApiRequest request)
    {
        if (request.AccountType is AccountEnums.AccountKeys.CASH)
            return CashAccount.New(
                new UserId(userId),
                new AccountName(request.Name),
                request.Description,
                request.DisplayColor,
                request.IsDefault,
                request.IsDefault ? 1 : null);

        if (request.AccountType is AccountEnums.AccountKeys.BANK)
        {
            if (request.BankAccountType is BankAccountEnums.BankAccountKeys.CHECKING)
                return CheckingAccount.New(
                    new UserId(userId),
                    new AccountName(request.Name),
                    request.Description,
                    request.DisplayColor,
                    new Money(request.OverdraftAmount),
                isDefault: request.IsDefault,
                    sortOrder: request.IsDefault ? 1 : null);

            if (request.BankAccountType is BankAccountEnums.BankAccountKeys.SAVINGS)
                return SavingsAccount.New(
                    new UserId(userId),
                    new AccountName(request.Name),
                    request.Description,
                    request.DisplayColor,
                    new PercentageRate(request.InterestRate),
                    isDefault: request.IsDefault,
                    sortOrder: request.IsDefault ? 1 : null);

            if (request.BankAccountType is BankAccountEnums.BankAccountKeys.CREDITLINE)
                return CreditLineAccount.New(
                    new UserId(userId),
                    new AccountName(request.Name),
                    request.Description,
                    request.DisplayColor,
                    new Money(request.CreditLimit),
                    new PercentageRate(request.InterestRate),
                    isDefault: request.IsDefault,
                    sortOrder: request.IsDefault ? 1 : null);

            throw new InvalidOperationException($"Bank Account Type `{request.AccountType}` is not supported");
        }

        if (request.AccountType is AccountEnums.AccountKeys.CREDITCARD)
            return CreditCardAccount.New(
                new UserId(userId),
                new AccountName(request.Name),
                request.Description,
                request.DisplayColor,
                new Money(request.CreditLimit),
                new PercentageRate(request.InterestRate),
                isDefault: request.IsDefault,
                sortOrder: request.IsDefault ? 1 : null);

        if (request.AccountType is AccountEnums.AccountKeys.INVESTMENT)
            throw new InvalidOperationException("Investment Account Type not supported");

        if (request.AccountType is AccountEnums.AccountKeys.LOAN)
            return LoanAccount.New(
                new UserId(userId),
                request.LoanAccountType,
                new AccountName(request.Name),
                request.Description,
                request.DisplayColor,
                new Money(request.CreditLimit),
                new PercentageRate(request.InterestRate),
                isDefault: request.IsDefault,
                sortOrder: request.IsDefault ? 1 : null);

        throw new InvalidOperationException($"Account Type `{request.AccountType}` is unknown");
    }

    internal sealed class Validator : AbstractValidator<CreateAccountCommand>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Request.UserId).NotNull();
            RuleFor(x => x).NotEmpty();
        }
    }
}