using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Accounts.Infrastructure.Mappers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data.Repositories;

/// <summary>
/// Responsible for managing the persistence of Account Documents.
/// </summary>
public sealed class AccountsRepository(IMongoDatabase mongoDb) :
    //XpnssDbRepository<AccountDocument>(new AccountsDbContext(mongoDb)),
    MongoDbRepository<AccountDocument>(new AccountsDbContext(mongoDb)),
    IAccountsRepository
{
    public async Task<Result<BaseAccount>> AddAsync(
        BaseAccount account,
        CancellationToken cancellationToken = default)
    {
        if (account is null)
            return Result.Fail("Account cannot be null");

        try
        {
            var accountDoc = InfrastructureMapper.Map(account);

            if (accountDoc is null)
                return Result.Fail("Could not map the Account to its Document");

            await AddDocumentAsync(accountDoc, cancellationToken);

            // Do this so we can update the State of the Account
            var newAccount = InfrastructureMapper.Map(accountDoc, true);

            if (newAccount is null)
                return Result.Fail("Could not map the Account");

            //HandleDomainEvents(account);

            return newAccount;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not save the Account{Environment.NewLine}{e.Message}");
        }
    }

    public async Task<Result<BaseAccount?>> GetAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
            accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{accountId}`");

        var accountDocument = await FirstOrDefaultDocumentAsync(a =>
                a.UserId.Equals(userObjectId) &&
                a.Id.Equals(accountObjectId),
                                cancellationToken);

        if (accountDocument is null)
            return Result.Ok<BaseAccount?>(null);

        var account = InfrastructureMapper.Map(accountDocument, true);

        if (account is null)
            return Result.Fail("Failed to map AccountDocument to Account");

        return Result.Ok<BaseAccount?>(account);
    }

    public async Task<Result<TType?>> GetAsync<TType>(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default) where TType : BaseAccount
    {
        var result = await GetAsync(userId, accountId, cancellationToken);

        if (result.IsFailed)
            return Result.Fail<TType?>(result.Errors);

        if (result.Value is null)
            return Result.Ok<TType?>(default);

        return Result.Ok((TType?)result.Value);
    }

    //public async Task<Result<ReadOnlyCollection<AdjustmentHistories>>> GetAdjustmentHistoryAsync(
    //    string userId,
    //    string accountId,
    //    string timeZone,
    //    CancellationToken cancellationToken = default)
    //{
    //    if (string.IsNullOrWhiteSpace(userId))
    //        return Result.Fail("UserId cannot be null or empty");

    //    if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
    //        accountObjectId.Equals(ObjectId.NewId))
    //        return Result.Fail($"Invalid AccountId: `{accountId}`");

    //    var doc = await FirstOrDefaultDocumentAsync(a =>
    //            a.Id.Equals(accountObjectId) &&
    //            a.UserId.Equals(userId) /*&& !a.IsClosed*/,
    //        cancellationToken);

    //    if (doc is null)
    //        return Result.Fail($"BaseAccount not found for AccountId: `{accountId}` " +
    //                           $"and UserId: `{userId}`");

    //    var changes = doc.AdjustmentHistories.AsEnumerable();

    //    return Result.Ok(changes.ToList().AsReadOnly());
    //}

    public async Task<Result<IEnumerable<BaseAccount>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        var docs = await FindDocumentsAsync(a =>
            a.UserId == userObjectId, cancellationToken);

        var payerPayees = InfrastructureMapper.Map(docs);

        return Result.Ok(payerPayees);

        // Projection
        //var matchBuilder = Builders<AccountDocument>.Filter;
        //var match = matchBuilder.Eq(a => a.UserId, userId);

        //var projectionBuilder = Builders<AccountDocument>.Projection;
        //var projection = projectionBuilder.Exclude(a => a.AdjustmentHistories).Exclude(a => a.MonthlyTotals);

        //var pipeline = new EmptyPipelineDefinition<AccountDocument>()
        //    .Match(match)
        //    .Project(projection);

        //var bsonDocs = (await Collection.Aggregate().Match(match).Project(projection).ToListAsync(cancellationToken)) ?? [];

        //if (!bsonDocs.Any())
        //    return Result.Ok(Enumerable.EmptyId<BaseAccount>());

        //var docs = bsonDocs.Select(bsonDoc => BsonSerializer.Deserialize<AccountDocument>(bsonDoc));

        //var accounts = InfrastructureMapper.Map(docs);

        //return Result.Ok(accounts);
    }

    public async Task<Result> UpdateAsync(BaseAccount updatedAccount, CancellationToken cancellationToken = default)
    {
        var accountDoc = InfrastructureMapper.Map(updatedAccount);

        if (accountDoc is null)
            return Result.Fail("Could not map the Account to its Document");

        var saveCount = await UpdateDocumentAsync(accountDoc, cancellationToken);

        if (saveCount == 0)
            return Result.Fail("Could not update the Account");

        //HandleIntegrationEvents(account);

        return Result.Ok();
    }

    //private void HandleIntegrationEvents(BaseAccount entity)
    //{
    //    if (_mediator is null)
    //        return;

    //    foreach (var @event in entity.DomainEvents)
    //    {
    //        //await _eventDispatcher.DispatchAsync(@event);
    //        _mediator.Publish(@event);
    //    }

    //    entity.ClearDomainEvents();
    //}
}