using System.Text.Json;
using System.Text.Json.Serialization;

using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Transactions.Application;

/// <summary>
/// Used to deserialize TransactionDto to the correct type when sending from the Api endpoint to the caller (external app)
/// </summary>
public class TransactionDtoJsonConverter : JsonConverter<TransactionDto?>
{
    public override TransactionDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        if (!jsonDoc.RootElement.TryGetProperty(nameof(CreateTransactionApiRequest.TransactionType), out var typeProp))
        {
            throw new JsonException();
        }

        var type = typeProp.GetString();
        switch (type)
        {

            case nameof(TransactionEnums.TransactionKeys.DEPOSIT):
                var deposit =
                    JsonSerializer.Deserialize<DepositTransactionDto>(
                        jsonDoc.RootElement.GetRawText(), options);

                return deposit;

            case nameof(TransactionEnums.TransactionKeys.PURCHASE):
                var purchase =
                    JsonSerializer.Deserialize<PurchaseTransactionDto>(
                        jsonDoc.RootElement.GetRawText(), options);

                return purchase;

            case nameof(TransactionEnums.TransactionKeys.WITHDRAWAL):
                var withdrawal =
                    JsonSerializer.Deserialize<WithdrawalTransactionDto>(
                        jsonDoc.RootElement.GetRawText(), options);

                return withdrawal;

            default:
                return
                    JsonSerializer.Deserialize<TransactionDto>(
                        jsonDoc.RootElement.GetRawText(), options);
        }
    }

    public override void Write(Utf8JsonWriter writer, TransactionDto value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        JsonSerializer.Serialize(writer, value, type, options);
    }
}

/// <summary>
/// Used to deserialize the CreateTransactionRequest from the caller (external app), to the correct type for the Api endpoint
/// </summary>
public class CreateTransactionRequestsJsonConverter : JsonConverter<CreateTransactionApiRequest?>
{
    public override CreateTransactionApiRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);

        var rawText = jsonDoc.RootElement.GetRawText();

        //if (!jsonDoc.RootElement.TryGetProperty(nameof(CreateTransactionApiRequest.IsCredit), out var isCreditProp))
        //{
        //    throw new JsonException();
        //}

        if (!jsonDoc.RootElement.TryGetProperty(nameof(CreateTransactionApiRequest.TransactionType), out var typeProp))
        {
            throw new JsonException();
        }

        var type = typeProp.GetInt32();
        switch (type)
        {
            case (int)TransactionEnums.TransactionKeys.DEPOSIT:
                {
                    var deposit =
                        JsonSerializer.Deserialize<CreateDepositTransactionApiRequest>(
                            jsonDoc.RootElement.GetRawText(), options);

                    return deposit;
                }

            case (int)TransactionEnums.TransactionKeys.PURCHASE:
                {
                    var purchase =
                        JsonSerializer.Deserialize<CreatePurchaseTransactionApiRequest>(
                            jsonDoc.RootElement.GetRawText(), options);

                    return purchase;
                }

            case (int)TransactionEnums.TransactionKeys.WITHDRAWAL:
                {
                    var withdrawal =
                        JsonSerializer.Deserialize<CreateWithdrawalTransactionApiRequest>(
                            jsonDoc.RootElement.GetRawText(), options);

                    return withdrawal;
                }

            default:
                throw new InvalidOperationException($"Transaction Type '{type}' is not yet supported");
        }
    }

    public override void Write(Utf8JsonWriter writer, CreateTransactionApiRequest? value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        JsonSerializer.Serialize(writer, value, type, options);
    }
}