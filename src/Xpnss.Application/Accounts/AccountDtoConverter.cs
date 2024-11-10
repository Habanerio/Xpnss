using System.Text.Json;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Application.Accounts.DTOs;
using Habanerio.Xpnss.Domain.Accounts;

namespace Habanerio.Xpnss.Application.Accounts;

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
                /*
                case nameof(AccountTypes.Keys.Cash):
                    return JsonSerializer.Deserialize<CashAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountTypes.Keys.Checking):
                    return JsonSerializer.Deserialize<CheckingAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountTypes.Keys.Savings):
                    return JsonSerializer.Deserialize<SavingsAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountTypes.Keys.CreditCard):
                    return JsonSerializer.Deserialize<CreditCardAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountTypes.Keys.LineOfCredit):
                    return JsonSerializer.Deserialize<LineOfCreditAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                */
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