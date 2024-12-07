using System.Text.Json;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Transactions.Application;

/// <summary>
/// Used to deserialize TransactionDto to the correct type when sending from the Api endpoint to the caller (external app)
/// </summary>
public class TransactionDtoJsonConverter : JsonConverter<TransactionDto?>
{
    public override TransactionDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        if (!jsonDoc.RootElement.TryGetProperty(nameof(CreateTransactionRequest.TransactionType), out var typeProp))
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
public class CreateTransactionRequestsJsonConverter : JsonConverter<CreateTransactionRequest?>
{
    public override CreateTransactionRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        if (!jsonDoc.RootElement.TryGetProperty(nameof(CreateTransactionRequest.TransactionType), out var typeProp))
        {
            throw new JsonException();
        }

        var type = typeProp.GetString();
        switch (type)
        {
            case nameof(TransactionEnums.TransactionKeys.DEPOSIT):
                {
                    var deposit =
                        JsonSerializer.Deserialize<CreateDepositTransactionRequest>(
                            jsonDoc.RootElement.GetRawText(), options);

                    return deposit;
                }

            case nameof(TransactionEnums.TransactionKeys.PURCHASE):
                {
                    var purchase =
                        JsonSerializer.Deserialize<CreatePurchaseTransactionRequest>(
                            jsonDoc.RootElement.GetRawText(), options);

                    return purchase;
                }

            default:
                return JsonSerializer.Deserialize<CreateTransactionRequest>(jsonDoc.RootElement.GetRawText(), options);
        }
    }

    public override void Write(Utf8JsonWriter writer, CreateTransactionRequest value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        JsonSerializer.Serialize(writer, value, type, options);
    }
}