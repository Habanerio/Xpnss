using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Transactions.Domain.Entities;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Habanerio.Xpnss.Transactions.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Transactions.Infrastructure.Mappers;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Transactions.Infrastructure.Data.Repositories;

/// <summary>
/// Responsible for managing the persistence of Transaction Documents.
/// </summary>
/// <param name="options">Db settings to connect to the Mongo Db</param>
/// <param name="mediator"></param>
public class TransactionsRepository(
    IOptions<MongoDbSettings> options,
    IMongoDatabase mongoDb,
    IMediator? mediator = null)
    : MongoDbRepository<TransactionDocument>(new TransactionsDbContext(mongoDb)), ITransactionsRepository
{
    private readonly IMediator? _mediator = mediator;

    public async Task<Result<TransactionBase>> AddAsync(
        TransactionBase transaction,
        CancellationToken cancellationToken = default)
    {
        if (transaction is null)
            return Result.Fail("Transaction cannot be null");

        try
        {
            var transactionDoc = InfrastructureMapper.Map(transaction);

            if (transactionDoc is null)
                return Result.Fail("Could not map the Transaction");

            await AddDocumentAsync(transactionDoc, cancellationToken);

            // Do this so we can update the State of the Transaction
            var updatedTransaction = InfrastructureMapper.Map(transactionDoc);

            if (updatedTransaction is null)
                return Result.Fail("Could not map the Transaction");

            //HandleDomainEvents(transaction);

            return updatedTransaction;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not save the Transaction{Environment.NewLine}{e.Message}");
        }
    }

    public async Task<Result<TransactionBase?>> GetAsync(
        string userId,
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(transactionId, out var transactionObjectId) ||
            transactionObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid TransactionId: `{transactionId}`");

        var doc = await FirstOrDefaultDocumentAsync(t =>
                                    t.Id.Equals(transactionObjectId) && t.UserId.Equals(userId),
                                cancellationToken);

        if (doc is null)
            return Result.Ok<TransactionBase?>(null);

        var transaction = InfrastructureMapper.Map(doc);

        if (transaction is null)
            return Result.Fail("Could not map the Transaction");

        return Result.Ok<TransactionBase?>(transaction);
    }

    public async Task<Result<IEnumerable<TransactionBase>>> ListAsync(
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

        var transactionDocs = await FindDocumentsAsync(t =>
            t.UserId.Equals(userId) &&
            (accountObjectId == null || t.AccountId.Equals(accountObjectId)) &&
            t.TransactionDate.Date >= newStartDate.Date &&
            t.TransactionDate.Date <= newEndDate.Date,
            cancellationToken);

        var transactions = InfrastructureMapper.Map(transactionDocs);

        return Result.Ok(transactions);
    }

    public async Task<Result<TransactionBase>> UpdateAsync(
        TransactionBase transaction,
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