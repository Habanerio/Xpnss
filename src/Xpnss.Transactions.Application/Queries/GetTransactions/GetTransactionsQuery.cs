using System.Text.Json.Serialization;
using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;

namespace Habanerio.Xpnss.Transactions.Application.Queries.GetTransactions;

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
