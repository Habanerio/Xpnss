using FluentResults;

namespace Habanerio.Xpnss.Transactions.Domain.Interfaces;

public interface ITransactionsRepository
{
    Task<Result<Transaction>> AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);

    Task<Result<Transaction?>> GetAsync(
        string userId,
        string transactionId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Transaction>>> ListAsync(
        string userId,
        string accountId = "",
        DateTime? startDate = null,
        DateTime? endDate = null,
        string userTimeZone = "",
        CancellationToken cancellationToken = default);

    Task<Result<Transaction>> UpdateAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);
}