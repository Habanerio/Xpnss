using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Shared.Requests.Transactions;

public record CopyTransactionRequest
{
    [Required]
    public int TransactionDateYear { get; set; }

    [Required]
    public int TransactionDateMonth { get; set; }

    [Required]
    public int TransactionDateDay { get; set; }

    public DateTime TransactionDate { get; set; }


    [JsonConstructor]
    public CopyTransactionRequest() { }

    public CopyTransactionRequest(string userId, string transactionId, DateTime transactionDate)
    {
        TransactionDateYear = transactionDate.Year;
        TransactionDateMonth = transactionDate.Month;
        TransactionDateDay = transactionDate.Day;
    }

    public CopyTransactionRequest(int transactionDateYear, int transactionDateMonth, int transactionDateDay, DateTimeKind? dtk = null)
    {
        TransactionDateYear = transactionDateYear;
        TransactionDateMonth = transactionDateMonth;
        TransactionDateDay = transactionDateDay;

        TransactionDate = new DateTime(
            TransactionDateYear,
            TransactionDateMonth,
            TransactionDateDay,
            0, 0, 0, dtk ?? DateTimeKind.Local);
    }
}