using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;

namespace Habanerio.Xpnss.Transactions.Application.Queries.GetTransaction;

public class GetTransactionQuery : ITransactionsQuery<Result<TransactionDto?>>
{
    public string UserId { get; set; }

    public string TransactionId { get; set; }

    public string TimeZone { get; set; }
}
