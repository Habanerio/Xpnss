using System.Text.Json;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Modules.Accounts.DTOs;

namespace Habanerio.Xpnss.Modules.Accounts.Common;

/// <summary>
/// Used to deserialize AccountDto to the correct type
/// </summary>
public class AccountDtoConverter : JsonConverter<AccountDto>
{
    public override AccountDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            if (!jsonDoc.RootElement.TryGetProperty("AccountType", out var typeProp))
            {
                throw new JsonException();
            }

            var type = typeProp.GetString();
            switch (type)
            {
                case nameof(AccountTypes.Cash):
                    return JsonSerializer.Deserialize<CashAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountTypes.Checking):
                    return JsonSerializer.Deserialize<CheckingAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountTypes.Savings):
                    return JsonSerializer.Deserialize<SavingsAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountTypes.CreditCard):
                    return JsonSerializer.Deserialize<CreditCardAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountTypes.LineOfCredit):
                    return JsonSerializer.Deserialize<LineOfCreditAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                default:
                    return JsonSerializer.Deserialize<AccountDto>(jsonDoc.RootElement.GetRawText(), options);
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, AccountDto value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        JsonSerializer.Serialize(writer, value, type, options);
    }
}