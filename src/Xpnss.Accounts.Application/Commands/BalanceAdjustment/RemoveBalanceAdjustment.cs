using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Accounts.Application.Commands.BalanceAdjustment;

public sealed record RemoveBalanceAdjustmentCommand
{
    public string UserId { get; set; } = "";

    public string AccountId { get; set; } = "";

    public string AdjustmentId { get; set; } = "";

    [JsonConstructor]
    public RemoveBalanceAdjustmentCommand() { }

    public RemoveBalanceAdjustmentCommand(string userId, string accountId, string adjustmentId)
    {
        UserId = userId;
        AccountId = accountId;
        AdjustmentId = adjustmentId;
    }
}