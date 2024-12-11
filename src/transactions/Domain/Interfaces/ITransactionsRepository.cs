using FluentResults;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

namespace Habanerio.Xpnss.Transactions.Domain.Interfaces;

public interface ITransactionsRepository
{
    Task<Result<Transaction>> AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Transaction>>> FindAsync(
        string userId,
        string accountId = "",
        DateTime? startDate = null,
        DateTime? endDate = null,
        string userTimeZone = "",
        CancellationToken cancellationToken = default);

    Task<Result<Transaction?>> GetAsync(
        string userId,
        string transactionId,
        CancellationToken cancellationToken = default);

    Task<Result<Transaction>> UpdateAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);
}