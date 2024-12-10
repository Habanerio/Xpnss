using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Shared.Requests;

public record CreateUserProfileApiRequest
{
    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string ExtUserId { get; set; } = string.Empty;

    [JsonPropertyName("DefaultCurrency")]
    [JsonConverter(typeof(JsonNumberEnumConverter<CurrencyEnums.CurrencyKeys>))]
    public CurrencyEnums.CurrencyKeys DefaultCurrency { get; set; } = CurrencyEnums.CurrencyKeys.CAD;

    [JsonConstructor]
    public CreateUserProfileApiRequest()
    { }

    public CreateUserProfileApiRequest(
        string email,
        string firstName,
        string lastName = "",
        string extUserId = "",
        CurrencyEnums.CurrencyKeys defaultCurrency = CurrencyEnums.CurrencyKeys.CAD)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ExtUserId = extUserId;
        DefaultCurrency = defaultCurrency;
    }
}