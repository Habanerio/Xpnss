using Habanerio.Xpnss.Application.DTOs;

namespace Habanerio.Xpnss.Apis.App.AppApis.Models.Transactions;

public record GetTransactionResponse(TransactionDto Transaction, MerchantDto? Merchant) :
    ApiResponse<(TransactionDto Transaction, MerchantDto? Merchant)>((Transaction, Merchant));