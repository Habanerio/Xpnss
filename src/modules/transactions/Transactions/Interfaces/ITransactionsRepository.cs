using FluentResults;
using Habanerio.Xpnss.Modules.Transactions.Data;

namespace Habanerio.Xpnss.Modules.Transactions.Interfaces;

public interface ITransactionsRepository
{
    Task<Result<TransactionDocument>> AddAsync(
        TransactionDocument transaction,
        CancellationToken cancellationToken = default);

    Task<Result<TransactionDocument>> GetByIdAsync(
        string userId,
        string transactionId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<TransactionDocument>>> ListAsync(
        string userId,
        string accountId = "",
        DateTime? startDate = null,
        DateTime? endDate = null,
        string userTimeZone = "",
        CancellationToken cancellationToken = default);
}