using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Habanerio.Xpnss.Transactions.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Transactions.Infrastructure.Mappers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Transactions.Infrastructure.Data.Repositories;

/// <summary>
/// Responsible for managing the persistence of Transaction Documents.
/// </summary>
public class TransactionsRepository(IMongoDatabase mongoDb)
    : MongoDbRepository<TransactionDocument>(new TransactionsDbContext(mongoDb)),
        ITransactionsRepository
{
    public async Task<Result<Transaction>> AddAsync(
        Transaction? transaction,
        CancellationToken cancellationToken = default)
    {
        if (transaction is null)
            return Result.Fail("Transaction cannot be null");

        try
        {
            var transactionDoc = InfrastructureMapper.Map(transaction);

            if (transactionDoc is null)
                return Result.Fail("Could not map the Transaction to TransactionDoc");

            await AddDocumentAsync(transactionDoc, cancellationToken);

            // Do this so we can update the State of the Transaction
            var newTransaction = InfrastructureMapper.Map(transactionDoc);

            if (newTransaction is null)
                return Result.Fail("Could not map the TransactionDoc to Transaction");

            //HandleDomainEvents(transaction);

            return newTransaction;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not save the Transaction{Environment.NewLine}{e.Message}");
        }
    }

    public async Task<Result<IEnumerable<Transaction>>> FindAsync(
        string userId,
        string accountId = "",
        DateTime? startDate = null,
        DateTime? endDate = null,
        string userTimeZone = "",
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        var newEndDate = string.IsNullOrWhiteSpace(userTimeZone) ?
            DateTime.UtcNow :
            TimeZoneInfo.ConvertTimeToUtc(endDate ??
                                          DateTime.Now,
                TimeZoneInfo.FindSystemTimeZoneById(userTimeZone));

        var newStartDate = string.IsNullOrWhiteSpace(userTimeZone) ?
            startDate ?? newEndDate.AddMonths(-1) :
            TimeZoneInfo.ConvertTimeToUtc(startDate ??
                                          newEndDate.AddMonths(-1),
                TimeZoneInfo.FindSystemTimeZoneById(userTimeZone));

        ObjectId? accountObjectId = !string.IsNullOrWhiteSpace(accountId) ?
            ObjectId.Parse(accountId) :
            null;


        var transactionDocs = (await FindDocumentsAsync(t =>
                t.UserId.Equals(userObjectId) &&
                (accountObjectId == null || t.AccountId.Equals(accountObjectId)) &&
                t.TransactionDate.Date >= newStartDate.Date &&
                t.TransactionDate.Date <= newEndDate.Date,
            cancellationToken))?
            .OrderBy(t => t.TransactionDate)
            .ToList() ?? [];

        if (!transactionDocs.Any())
            return Result.Ok<IEnumerable<Transaction>>(new List<Transaction>());

        var transactions = InfrastructureMapper.Map(transactionDocs);

        return Result.Ok(transactions);
    }

    public async Task<Result<Transaction?>> GetAsync(
        string userId,
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        if (!ObjectId.TryParse(transactionId, out var transactionObjectId) ||
            transactionObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid TransactionId: `{transactionId}`");

        var doc = await FirstOrDefaultDocumentAsync(t =>
                t.UserId.Equals(userObjectId) &&
                t.Id.Equals(transactionObjectId),
            cancellationToken);

        if (doc is null)
            return Result.Ok<Transaction?>(null);

        var transaction = InfrastructureMapper.Map(doc);

        if (transaction is null)
            throw new InvalidOperationException($"{nameof(GetType)}: Could not map the transaction from a Document to an Entity");

        return Result.Ok<Transaction?>(transaction);
    }

    public async Task<Result<Transaction>> UpdateAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        var existingTransaction = await GetAsync(
                transaction.UserId,
                transaction.Id,
            cancellationToken);

        if (existingTransaction.IsFailed || existingTransaction.ValueOrDefault is null)
            return Result.Fail(existingTransaction.Errors[0].Message ??
                $"Could not find Transaction #{transaction.Id} for User {transaction.UserId}");

        var transactionDoc = InfrastructureMapper.Map(transaction);

        if (transactionDoc is null)
            return Result.Fail("Could not map the Transaction");

        var saveCount = await UpdateDocumentAsync(transactionDoc, cancellationToken);

        if (saveCount == 0)
            return Result.Fail($"Could not update Transaction #{transaction.Id} for User {transaction.UserId}");

        //HandleDomainEvents(transaction);

        return transaction;
    }

    //private void HandleIntegrationEvents(Transaction entity)
    //{
    //    if (_mediator is null)
    //        return;

    //    foreach (var @event in entity.DomainEvents)
    //    {
    //        //await _eventDispatcher.DispatchAsync(@event);
    //        _mediator.Send(@event);
    //    }

    //    entity.ClearDomainEvents();
    //}
}