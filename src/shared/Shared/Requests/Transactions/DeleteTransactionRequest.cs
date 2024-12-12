using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Shared.Requests.Transactions;

public record DeleteTransactionRequest : UserRequiredRequest
{
    [Required]
    public string AccountId { get; set; } = string.Empty;


    [Required]
    public string TransactionId { get; set; } = string.Empty;

    [JsonConstructor]
    public DeleteTransactionRequest() { }

    public DeleteTransactionRequest(string userId, string accountId, string transactionId)
    {
        UserId = userId;
        AccountId = accountId;
        TransactionId = transactionId;
    }
}