using System.Text.Json.Serialization;
using FluentResults;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;

namespace Habanerio.Xpnss.Application.Accounts.Commands.AdjustBalance;

/// <summary>
/// This is for manual changes to the Account Balance, which is logged.
/// 
/// Do not use this for regular updates to the balance based on transactions.
/// 
/// This should have its own Api endpoint.
/// </summary>
public sealed record AdjustBalanceCommand : IAccountsCommand<Result<decimal>>
{
    public string UserId { get; set; } = "";

    public string AccountId { get; set; } = "";

    public decimal Balance { get; set; } = 0;

    public DateTime DateOfChange { get; set; }

    public string Reason { get; set; } = "";

    [JsonConstructor]
    public AdjustBalanceCommand() { }

    public AdjustBalanceCommand(string userId, string accountId, decimal balance, DateTime dateOfChange, string reason = "")
    {
        UserId = userId;
        AccountId = accountId;
        Balance = balance;
        DateOfChange = dateOfChange;
        Reason = reason;
    }
}