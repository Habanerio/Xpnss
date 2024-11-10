using System.Text.Json.Serialization;
using FluentResults;
using Habanerio.Xpnss.Application.Transactions.DTOs;
using Habanerio.Xpnss.Domain.Transactions.Interfaces;

namespace Habanerio.Xpnss.Application.Transactions.Queries.GetTransactions;

public class GetTransactionsQuery : ITransactionsQuery<Result<IEnumerable<TransactionDto>>>
{
    public string UserId { get; set; }

    public string AccountId { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string TimeZone { get; set; }

    [JsonConstructor]
    public GetTransactionsQuery(string userId, string accountId, DateTime? fromDate, DateTime? toDate, string timeZone = "")
    {
        UserId = userId;
        AccountId = accountId;
        FromDate = fromDate;
        ToDate = toDate;
        TimeZone = timeZone;
    }

    public GetTransactionsQuery(string userId, string accountId)
    {
        UserId = userId;
        AccountId = accountId;
    }
}
