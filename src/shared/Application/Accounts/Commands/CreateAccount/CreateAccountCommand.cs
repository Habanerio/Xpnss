using System.ComponentModel.DataAnnotations;
using FluentResults;
using Habanerio.Xpnss.Application.Accounts.DTOs;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;

namespace Habanerio.Xpnss.Application.Accounts.Commands.CreateAccount;

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
    decimal OverDraftAmount = 0,
    string DisplayColor = "") : IAccountsCommand<Result<AccountDto>>;