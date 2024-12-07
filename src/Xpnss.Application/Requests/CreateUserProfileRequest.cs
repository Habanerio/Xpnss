using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.Requests;

public record CreateUserProfileRequest
{
    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string ExtUserId { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CurrencyEnums.CurrencyKeys DefaultCurrency { get; set; } = CurrencyEnums.CurrencyKeys.CAD;

    [JsonConstructor]
    public CreateUserProfileRequest()
    { }

    public CreateUserProfileRequest(
        string email,
        string firstName,
        string extUserId,
        string lastName = "",
        CurrencyEnums.CurrencyKeys defaultCurrency = CurrencyEnums.CurrencyKeys.CAD)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ExtUserId = extUserId;
        DefaultCurrency = defaultCurrency;
    }
}