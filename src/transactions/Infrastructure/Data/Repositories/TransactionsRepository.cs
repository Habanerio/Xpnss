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

            try
            {
                await AddDocumentAsync(transactionDoc, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

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

    //TODO: Need a PagedResults<T>
    public async Task<Result<(IEnumerable<Transaction> Results, int PageNo, int PageSize, int TotalPages, int TotalCount)>> SearchAsync(
        string userId,
        string forAccountId = "",
        string forCategoryId = "",
        string forPayerPayeeId = "",
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNo = 1,
        int pageSize = 100,
        string userTimeZone = "",
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        if (userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: UserId cannot be empty");

        ObjectId? accountObjectId = !string.IsNullOrWhiteSpace(forAccountId) ?
            ObjectId.Parse(forAccountId) :
            null;

        ObjectId? categoryObjectId = !string.IsNullOrWhiteSpace(forCategoryId) ?
            ObjectId.Parse(forCategoryId) :
            null;

        ObjectId? payerPayeeObjectId = !string.IsNullOrWhiteSpace(forPayerPayeeId) ?
            ObjectId.Parse(forPayerPayeeId) :
            null;

        DateTime newToDate = toDate ?? DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(userTimeZone))
        {
            newToDate = TimeZoneInfo.ConvertTimeToUtc(
                newToDate,
                TimeZoneInfo.FindSystemTimeZoneById(userTimeZone));
        }

        DateTime newFromDate = fromDate ?? DateTime.UtcNow.AddMonths(-1);
        if (!string.IsNullOrWhiteSpace(userTimeZone))
        {
            newFromDate = TimeZoneInfo.ConvertTimeToUtc(
                newFromDate,
                TimeZoneInfo.FindSystemTimeZoneById(userTimeZone));
        }

        var transactionResults = (await FindDocumentsAsync(t => (
                t.UserId.Equals(userObjectId) &&

                (accountObjectId == null || t.AccountId.Equals(accountObjectId)) &&
                (categoryObjectId == null || t.CategoryId.Equals(categoryObjectId)) &&
                (payerPayeeObjectId == null || t.PayerPayeeId.Equals(payerPayeeObjectId)) &&

                (
                    t.TransactionDate.Date >= newFromDate.Date && t.TransactionDate.Date <= newToDate.Date
                )),

            pageNo,
            pageSize,
            true,
            t => t.TransactionDate,
            cancellationToken));

        var transactionDocs = transactionResults.Results?.ToList() ?? [];

        if (!transactionDocs.Any())
            return Result.Ok<(IEnumerable<Transaction> Results, int PageNo, int PageSize, int TotalPages, int TotalCount)>
                (([], pageNo, pageSize, 0, 0));

        var transactions = InfrastructureMapper.Map(transactionDocs);

        var results = (
            transactions,
            pageNo,
            pageSize,
            transactionResults.TotalPages,
            transactionResults.TotalCount);

        return Result.Ok(results);
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