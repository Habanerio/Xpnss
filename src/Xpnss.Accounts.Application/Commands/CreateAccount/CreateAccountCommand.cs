using System.ComponentModel.DataAnnotations;
using FluentResults;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Application.DTOs;

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
    string DisplayColor = "") : IAccountsCommand<Result<AccountDto>>;