using System.Text.Json.Serialization;
using FluentResults;
using Habanerio.Xpnss.Application.Accounts.DTOs;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;

namespace Habanerio.Xpnss.Application.Accounts.Commands.UpdateAccountDetails;

public sealed record UpdateAccountDetailsCommand : IAccountsCommand<Result<AccountDto>>
{
    public string UserId { get; init; } = "";
    public string AccountId { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string DisplayColor { get; init; } = "";

    [JsonConstructor]
    public UpdateAccountDetailsCommand() { }

    public UpdateAccountDetailsCommand(string userId, string accountId, string name, string description, string displayColor)
    {
        UserId = userId;
        AccountId = accountId;
        Name = name;
        Description = description;
        DisplayColor = displayColor;
    }
}