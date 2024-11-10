using FluentResults;
using Habanerio.Xpnss.Application.Transactions.DTOs;
using Habanerio.Xpnss.Domain.Transactions.Interfaces;

namespace Habanerio.Xpnss.Application.Transactions.Queries.GetTransaction;

public class GetTransactionQuery : ITransactionsQuery<Result<TransactionDto?>>
{
    public string UserId { get; set; }

    public string TransactionId { get; set; }

    public string TimeZone { get; set; }
}
