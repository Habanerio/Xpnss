using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Shared.Requests;

public record UpdateAccountDetailsApiRequest : UserRequiredApiRequest
{
    public string AccountId { get; init; }

    public string? Name { get; set; } = null;

    public string? Description { get; set; } = null;

    public string? DisplayColor { get; set; } = null;

    public bool? IsDefault { get; set; } = null;

    public int? SortOrder { get; set; } = null;

    [JsonConstructor]
    public UpdateAccountDetailsApiRequest() { }

    public UpdateAccountDetailsApiRequest(
        string userId,
        string accountId,
        string? name = null,
        string? description = null,
        string? displayColor = null,
        bool? isDefault = null,
        int? sortOrder = null)
    {
        UserId = userId;
        AccountId = accountId;
        Name = name;
        Description = description;
        DisplayColor = displayColor;
        IsDefault = isDefault;
        SortOrder = sortOrder;
    }
}