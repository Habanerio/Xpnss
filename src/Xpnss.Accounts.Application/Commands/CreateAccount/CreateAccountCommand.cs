using System.ComponentModel.DataAnnotations;
using FluentResults;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Accounts.Application.Commands.CreateAccount;

/// <summary>
/// MediatR Command to Create a new Account
/// </summary>
/// <remarks>
/// Needs to be cleaned up
/// </remarks>
public sealed record CreateAccountCommand(
    [Required] string UserId,
    [Required] string AccountType,
    [Required] string Name,
    //decimal Balance = 0,
    string Description = "",
    decimal CreditLimit = 0,
    decimal InterestRate = 0,
    decimal OverdraftAmount = 0,
    string DisplayColor = "") :
    IAccountsCommand<Result<AccountDto>>
{
    public static CreateAccountCommand NewCashAccount(
        string userId,
        string name,
        string description,
        string displayColor) => new CreateAccountCommand(
            userId, AccountTypes.Keys.CASH.ToString(),
            name,
            description,
            0,
            0,
            0,
            displayColor);

    public static CreateAccountCommand NewCheckingAccount(
        string userId,
        string name,
        string description,
        string displayColor,
        decimal overdraftAmount) => new CreateAccountCommand(
            userId, AccountTypes.Keys.CHECKING.ToString(),
            name,
            description,
            0,
            0,
            overdraftAmount,
            displayColor);

    public static CreateAccountCommand NewSavingsAccount(
        string userId,
        string name,
        string description,
        decimal interestRate,
        string displayColor) => new CreateAccountCommand(
            userId, AccountTypes.Keys.SAVINGS.ToString(),
            name,
            description,
            0,
            interestRate,
            0,
            displayColor);

    public static CreateAccountCommand NewCreditCardAccount(
        string userId,
        string name,
        string description,
        decimal creditLimit,
        decimal interestRate,
        string displayColor) => new CreateAccountCommand(
            userId, AccountTypes.Keys.CREDIT_CARD.ToString(),
            name,
            description,
            creditLimit,
            interestRate,
            0,
            displayColor);

    public static CreateAccountCommand NewLineOfCreditAccount(
        string userId,
        string name,
        string description,
        decimal creditLimit,
        decimal interestRate,
        string displayColor) => new CreateAccountCommand(
            userId, AccountTypes.Keys.LINE_OF_CREDIT.ToString(),
            name,
            description,
            creditLimit,
            interestRate,
            0,
            displayColor);
}