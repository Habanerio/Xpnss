using FluentResults;
using Habanerio.Xpnss.Transactions.Domain.Entities;

namespace Habanerio.Xpnss.Transactions.Domain.Interfaces;

public interface ITransactionsRepository
{
    Task<Result<TransactionBase>> AddAsync(
        TransactionBase transaction,
        CancellationToken cancellationToken = default);

    Task<Result<TransactionBase?>> GetAsync(
        string userId,
        string transactionId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<TransactionBase>>> ListAsync(
        string userId,
        string accountId = "",
        DateTime? startDate = null,
        DateTime? endDate = null,
        string userTimeZone = "",
        CancellationToken cancellationToken = default);

    Task<Result<TransactionBase>> UpdateAsync(
        TransactionBase transaction,
        CancellationToken cancellationToken = default);
}