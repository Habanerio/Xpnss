using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Domain.Transactions;
using Habanerio.Xpnss.Domain.Transactions.Interfaces;
using Habanerio.Xpnss.Infrastructure.Documents;
using Habanerio.Xpnss.Infrastructure.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Infrastructure.Repositories;

public class TransactionsRepository(IOptions<MongoDbSettings> options)
    : MongoDbRepository<TransactionDocument>(new XpnssDbContext(options)), ITransactionsRepository
{
    public async Task<Result<Transaction>> AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        if (transaction is null)
            return Result.Fail("Transaction cannot be null");

        try
        {
            var transactionDoc = Mapper.Map(transaction);

            if (transactionDoc is null)
                return Result.Fail("Could not map the Transaction");

            transactionDoc.DateCreated = DateTime.UtcNow;

            await AddDocumentAsync(transactionDoc, cancellationToken);

            var newTransaction = Mapper.Map(transactionDoc);

            if (newTransaction is null)
                return Result.Fail("Could not map the Transaction");

            return newTransaction;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not save the Transaction{Environment.NewLine}{e.Message}");
        }
    }

    public async Task<Result<Transaction?>> GetAsync(
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
            return Result.Ok<Transaction?>(null);

        var transaction = Mapper.Map(doc);

        if (transaction is null)
            return Result.Fail("Could not map the Transaction");

        return Result.Ok<Transaction?>(transaction);
    }

    public async Task<Result<IEnumerable<Transaction>>> ListAsync(
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

        var transactionDocs = await FindAsync(t =>
            t.UserId.Equals(userId) &&
            (accountObjectId == null || t.AccountId.Equals(accountObjectId)) &&
            t.TransactionDate.Date >= newStartDate.Date &&
            t.TransactionDate.Date <= newEndDate.Date,
            cancellationToken);

        var transactions = Mapper.Map(transactionDocs);

        return Result.Ok(transactions);
    }
}