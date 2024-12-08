using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Application.Requests;

public record UpdateAccountDetailsApiRequest : UserRequiredApiRequest
{
    public string AccountId { get; } = string.Empty;

    public string Name { get; } = string.Empty;

    public string Description { get; } = string.Empty;

    public string DisplayColor { get; } = string.Empty;

    [JsonConstructor]
    public UpdateAccountDetailsApiRequest() { }

    public UpdateAccountDetailsApiRequest(
        string userId,
        string accountId,
        string name,
        string description,
        string displayColor)
    {
        UserId = userId;
        AccountId = accountId;
        Name = name;
        Description = description;
        DisplayColor = displayColor;
    }
}