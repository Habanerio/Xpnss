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

    public async Task<Result<TransactionDocument>> AddAsync(
        TransactionDocument transaction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            transaction.DateCreated = DateTime.UtcNow;

            await base.AddDocumentAsync(transaction, cancellationToken);

            return transaction;
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
        DateTime? startDate = null,
        DateTime? endDate = null,
        string userTimeZone = "",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var newEndDate = string.IsNullOrWhiteSpace(userTimeZone) ?
            DateTime.UtcNow :
            TimeZoneInfo.ConvertTimeToUtc(endDate ?? DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(userTimeZone));

        var newStartDate = string.IsNullOrWhiteSpace(userTimeZone) ?
            startDate ?? newEndDate.AddMonths(-1) :
            TimeZoneInfo.ConvertTimeToUtc(startDate ?? newEndDate.AddMonths(-1), TimeZoneInfo.FindSystemTimeZoneById(userTimeZone));

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