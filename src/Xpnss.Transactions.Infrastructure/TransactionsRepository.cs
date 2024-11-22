using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Transactions.Domain;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Transactions.Infrastructure;

/// <summary>
/// Responsible for managing the persistence of Transaction Documents.
/// </summary>
/// <param name="options">Db settings to connect to the Mongo Db</param>
/// <param name="mediator"></param>
public class TransactionsRepository(
    IOptions<MongoDbSettings> options,
    IMediator? mediator = null)
    : MongoDbRepository<TransactionDocument>(new TransactionsDbContext(options)), ITransactionsRepository
{
    private readonly IMediator? _mediator = mediator;

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

            await AddDocumentAsync(transactionDoc, cancellationToken);

            //HandleDomainEvents(transaction);

            return transaction;
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

        var doc = await FirstOrDefaultDocumentAsync(t =>
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

        var transactionDocs = await FindDocumentsAsync(t =>
            t.UserId.Equals(userId) &&
            (accountObjectId == null || t.AccountId.Equals(accountObjectId)) &&
            t.TransactionDate.Date >= newStartDate.Date &&
            t.TransactionDate.Date <= newEndDate.Date,
            cancellationToken);

        var transactions = Mapper.Map(transactionDocs);

        return Result.Ok(transactions);
    }

    public async Task<Result<Transaction>> UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var existingTransaction = await GetAsync(
            transaction.UserId,
            transaction.Id,
            cancellationToken);

        if (existingTransaction.IsFailed || existingTransaction.ValueOrDefault is null)
            return Result.Fail(existingTransaction.Errors[0].Message ??
                               $"Could not find Transaction #{transaction.Id} for User {transaction.UserId}");

        var transactionDoc = Mapper.Map(transaction);

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