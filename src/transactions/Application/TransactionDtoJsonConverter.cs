using System.Text.Json;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.DTOs.Transactions;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Transactions.Application;

/// <summary>
/// Used to deserialize TransactionDto to the correct type when sending from the Api endpoint to the caller (external app)
/// </summary>
public class TransactionDtoJsonConverter : JsonConverter<TransactionDto?>
{
    public override TransactionDto? Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        if (!jsonDoc.RootElement.TryGetProperty(
                nameof(CreateTransactionRequest.TransactionType), out var typeProp))
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

            case nameof(TransactionEnums.TransactionKeys.PAYMENT):
                var payment =
                    JsonSerializer.Deserialize<PaymentTransactionDto>(
                        jsonDoc.RootElement.GetRawText(), options);

                return payment;

            case nameof(TransactionEnums.TransactionKeys.PURCHASE):
                var purchase =
                    JsonSerializer.Deserialize<PurchasesTransactionDto>(
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

    public override void Write(Utf8JsonWriter writer, TransactionDto? value, JsonSerializerOptions options)
    {
        var type = value?.GetType();
        if (type != null)
        {
            JsonSerializer.Serialize(writer, value, type, options);
        }
    }
}

/// <summary>
/// Used to deserialize the CreateTransactionRequest from the caller (external app), to the correct type for the Api endpoint
/// </summary>
public class CreateTransactionRequestsJsonConverter : JsonConverter<CreateTransactionRequest?>
{
    public override CreateTransactionRequest? Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);

        var rawText = jsonDoc.RootElement.GetRawText();

        //if (!jsonDoc.RootElement.TryGetProperty(nameof(CreateTransactionApiRequest.IsCredit), out var isCreditProp))
        //{
        //    throw new JsonException();
        //}

        if (!jsonDoc.RootElement.TryGetProperty(
                nameof(CreateTransactionRequest.TransactionType), out var typeProp))
        {
            throw new JsonException();
        }

        if (!jsonDoc.RootElement.TryGetProperty("isCredit", out var isCreditProp))
        {
            throw new JsonException();
        }


        var isCredit = isCreditProp.GetBoolean();
        var type = typeProp.GetInt32();

        if (isCredit)
        {
            switch (type)
            {
                case (int)TransactionEnums.TransactionKeys.PAYMENT_IN:
                    {
                        var payment =
                            JsonSerializer.Deserialize<CreatePaymentInTransactionRequest>(
                                jsonDoc.RootElement.GetRawText(), options);

                        return payment;
                    }

                default:
                    {
                        var credit =
                            JsonSerializer.Deserialize<CreateCreditTransactionRequest>(
                                jsonDoc.RootElement.GetRawText(), options);

                        return credit;
                    }
            }
        }
        else
        {
            switch (type)
            {
                case (int)TransactionEnums.TransactionKeys.PAYMENT_OUT:
                    {
                        var payment =
                            JsonSerializer.Deserialize<CreatePaymentOutTransactionRequest>(
                                jsonDoc.RootElement.GetRawText(), options);

                        return payment;
                    }

                case (int)TransactionEnums.TransactionKeys.PURCHASE:
                    {
                        var purchase =
                            JsonSerializer.Deserialize<CreatePurchasesTransactionRequest>(
                                jsonDoc.RootElement.GetRawText(), options);

                        return purchase;
                    }

                default:
                    {
                        var debit =
                            JsonSerializer.Deserialize<CreateDebitTransactionRequest>(
                                jsonDoc.RootElement.GetRawText(), options);

                        return debit;
                    }
            }
        }


        //switch (type)
        //{
        //    case (int)TransactionEnums.TransactionKeys.DEPOSIT:
        //        {
        //            var deposit =
        //                JsonSerializer.Deserialize<CreateDepositTransactionRequest>(
        //                    jsonDoc.RootElement.GetRawText(), options);

        //            return deposit;
        //        }

        //    case (int)TransactionEnums.TransactionKeys.PAYMENT_OUT:
        //        {
        //            var payment =
        //                JsonSerializer.Deserialize<CreatePaymentOutTransactionRequest>(
        //                    jsonDoc.RootElement.GetRawText(), options);

        //            return payment;
        //        }

        //    case (int)TransactionEnums.TransactionKeys.PAYMENT_IN:
        //        {
        //            var payment =
        //                JsonSerializer.Deserialize<CreatePaymentInTransactionRequest>(
        //                    jsonDoc.RootElement.GetRawText(), options);

        //            return payment;
        //        }

        //    case (int)TransactionEnums.TransactionKeys.PURCHASE:
        //        {
        //            var purchase =
        //                JsonSerializer.Deserialize<CreatePurchasesTransactionRequest>(
        //                    jsonDoc.RootElement.GetRawText(), options);

        //            return purchase;
        //        }

        //    case (int)TransactionEnums.TransactionKeys.WITHDRAWAL:
        //        {
        //            var withdrawal =
        //                JsonSerializer.Deserialize<CreateWithdrawalTransactionRequest>(
        //                    jsonDoc.RootElement.GetRawText(), options);

        //            return withdrawal;
        //        }


        //    case (int)TransactionEnums.TransactionKeys.ADJUSTMENT_CREDIT:
        //        {
        //            var payment =
        //                JsonSerializer.Deserialize<CreateCreditTransactionRequest>(
        //                    jsonDoc.RootElement.GetRawText(), options);

        //            return payment;
        //        }

        //    //INTEREST_CHARGE

        //    case (int)TransactionEnums.TransactionKeys.BALANCE_TRANSFER_OUT:
        //        {
        //            var payment =
        //                JsonSerializer.Deserialize<CreateDebitTransactionRequest>(
        //                    jsonDoc.RootElement.GetRawText(), options);

        //            return payment;
        //        }

        //    case (int)TransactionEnums.TransactionKeys.INTEREST_CHARGE:
        //        {
        //            var payment =
        //                JsonSerializer.Deserialize<CreateDebitTransactionRequest>(
        //                    jsonDoc.RootElement.GetRawText(), options);

        //            return payment;
        //        }

        //    case (int)TransactionEnums.TransactionKeys.CHARGE_FEE:
        //        {
        //            var payment =
        //                JsonSerializer.Deserialize<CreateDebitTransactionRequest>(
        //                    jsonDoc.RootElement.GetRawText(), options);

        //            return payment;
        //        }




        //    default:
        //        throw new InvalidOperationException($"Transaction Type '{type}' is not yet supported");
        //}
    }

    public override void Write(
        Utf8JsonWriter writer, CreateTransactionRequest? value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        JsonSerializer.Serialize(writer, value, type, options);
    }
}