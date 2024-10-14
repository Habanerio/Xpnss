using FluentResults;
using Habanerio.Xpnss.Modules.Transactions.Data;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Transactions.Interfaces;

public interface ITransactionsRepository
{
    Task<Result<ObjectId>> AddAsync(TransactionDocument transaction, CancellationToken cancellationToken = default);

    Task<Result<TransactionDocument>> GetByIdAsync(
        string userId,
        string transactionId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<TransactionDocument>>> ListAsync(
        string userId,
        string accountId = "",
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        CancellationToken cancellationToken = default);
}