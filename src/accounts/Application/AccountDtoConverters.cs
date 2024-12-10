using System.Text.Json;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Accounts.Application;

/// <summary>
/// Used to deserialize AccountDto to the correct type
/// </summary>
public class AccountDtoConverters : JsonConverter<AccountDto>
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
                case nameof(AccountEnums.CurrencyKeys.Cash):
                    return JsonSerializer.Deserialize<CashAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountEnums.CurrencyKeys.Checking):
                    return JsonSerializer.Deserialize<CheckingAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountEnums.CurrencyKeys.Savings):
                    return JsonSerializer.Deserialize<SavingsAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountEnums.CurrencyKeys.CreditCard):
                    return JsonSerializer.Deserialize<CreditCardAccountDto>(jsonDoc.RootElement.GetRawText(), options);
                case nameof(AccountEnums.CurrencyKeys.LineOfCredit):
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

public class AccountRequestJsonConverter : JsonConverter<CreateAccountApiRequest>
{
    public override CreateAccountApiRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //TODO: Look at how I did it in TransactionCommandHandler
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            if (!(jsonDoc.RootElement.TryGetProperty("AccountType", out var accountTypeProp) &&
                jsonDoc.RootElement.TryGetProperty("BankAccountType", out var bankTypeProp) &&
                jsonDoc.RootElement.TryGetProperty("InvestmentAccountType", out var investmentTypeProp) &&
                jsonDoc.RootElement.TryGetProperty("LoanAccountType", out var loanTypeProp)))
            {
                throw new JsonException();
            }

            var accountType = accountTypeProp.GetInt32();
            var bankType = bankTypeProp.GetInt32();
            var investmentType = investmentTypeProp.GetInt32();
            var loanType = loanTypeProp.GetInt32();

            if (accountType == (int)AccountEnums.AccountKeys.CASH)
            {
                return JsonSerializer.Deserialize<CreateCashAccountRequest>(jsonDoc.RootElement.GetRawText(), options);
            }

            if (accountType == (int)AccountEnums.AccountKeys.BANK)
            {
                if (bankType == (int)BankAccountEnums.BankAccountKeys.CHECKING)
                {
                    return JsonSerializer.Deserialize<CreateCheckingAccountRequest>(jsonDoc.RootElement.GetRawText(), options);
                }

                if (bankType == (int)BankAccountEnums.BankAccountKeys.SAVINGS)
                {
                    return JsonSerializer.Deserialize<CreateSavingsAccountRequest>(jsonDoc.RootElement.GetRawText(), options);
                }

                if (bankType == (int)BankAccountEnums.BankAccountKeys.CREDITLINE)
                {
                    return JsonSerializer.Deserialize<CreateCreditLineAccountRequest>(jsonDoc.RootElement.GetRawText(), options);
                }

                throw new InvalidOperationException();
            }

            if (accountType == (int)AccountEnums.AccountKeys.CREDITCARD)
            {
                return JsonSerializer.Deserialize<CreateCreditCardAccountRequest>(jsonDoc.RootElement.GetRawText(), options);
            }


            if (accountType == (int)AccountEnums.AccountKeys.LOAN)
            {
                return JsonSerializer.Deserialize<CreateLoanAccountRequest>(jsonDoc.RootElement.GetRawText(), options);
            }

            throw new InvalidOperationException();
        }
    }

    public override void Write(Utf8JsonWriter writer, CreateAccountApiRequest value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        JsonSerializer.Serialize(writer, value, type, options);
    }
}