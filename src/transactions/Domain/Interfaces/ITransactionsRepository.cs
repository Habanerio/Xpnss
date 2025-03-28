using FluentResults;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

namespace Habanerio.Xpnss.Transactions.Domain.Interfaces;

public interface ITransactionsRepository
{
    Task<Result<Transaction>> AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);

    //TODO: Return a PagedResults<T>
    //TODO: Pass in the SearchRequest
    Task<Result<(IEnumerable<Transaction> Results, int PageNo, int PageSize, int TotalPages, int TotalCount)>>
        SearchAsync(
        string userId,
        string forAccountId = "",
        string forCategoryId = "",
        string forPayerPayeeId = "",
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNo = 1,
        int pageSize = 100,
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