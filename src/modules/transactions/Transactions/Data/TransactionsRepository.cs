using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Transactions.Data;

public class TransactionsRepository : MongoDbRepository<TransactionDocument>, ITransactionsRepository
{
    public TransactionsRepository(IOptions<MongoDbSettings> options) :
        base(new TransactionsDbContext(options))
    { }

    public async Task<Result<ObjectId>> AddAsync(
        TransactionDocument transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await base.AddDocumentAsync(transaction, cancellationToken);

            return transaction.Id;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not save the Transaction{Environment.NewLine}{e.Message}");
        }
    }

    public async Task<Result<TransactionDocument>> GetByIdAsync(
        string userId,
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(transactionId, out var transactionObjectId) ||
            transactionObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid TransactionId: `{transactionId}`");

        var doc = await FirstOrDefaultAsync(t =>
                                    t.Id.Equals(transactionObjectId) && t.UserId.Equals(userId),
                                cancellationToken);

        if (doc is null)
            return Result.Fail($"Transaction not found for TransactionId: `{transactionId}` " +
                               $"and UserId: `{userId}`");

        return Result.Ok(doc);
    }

    public async Task<Result<IEnumerable<TransactionDocument>>> ListAsync(
        string userId,
        string accountId = "",
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var newEndDate = endDate ?? DateTimeOffset.UtcNow;
        var newStartDate = startDate ?? newEndDate.AddMonths(-1);

        ObjectId? accountObjectId = !string.IsNullOrWhiteSpace(accountId) ?
            ObjectId.Parse(accountId) :
            null;

        var transactions = await FindAsync(t =>
            t.UserId.Equals(userId) &&
            (accountObjectId == null || t.AccountId.Equals(accountObjectId)) &&
            t.TransactionDate.Date >= newStartDate.Date &&
            t.TransactionDate.Date <= newEndDate.Date,
            cancellationToken);

        return Result.Ok(transactions);
    }
}