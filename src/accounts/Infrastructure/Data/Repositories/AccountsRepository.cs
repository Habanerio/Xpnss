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
    MongoDbRepository<AccountDocument>(new AccountsDbContext(mongoDb)),
    IAccountsRepository
{
    public async Task<Result<AbstractAccountBase>> AddAsync(
        AbstractAccountBase account,
        CancellationToken cancellationToken = default)
    {
        if (account is null)
            return Result.Fail("Cannot Update a null Account");

        if (!ObjectId.TryParse(account.UserId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{account.UserId}`");
        //throw new InvalidOperationException($"Add Account: UserId is ({Account.UserId}) is invalid");

        try
        {
            var accountDoc = InfrastructureMapper.Map(account);

            if (accountDoc is null)
                return Result.Fail("Could not map the Account to its Document");

            await AddDocumentAsync(accountDoc, cancellationToken);

            // Do this so we can update the State of the Account
            var newAccount = InfrastructureMapper.Map(accountDoc);

            if (newAccount is null)
                return Result.Fail("Could not map the Account");

            //HandleDomainEvents(Account);

            return newAccount;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not save the Account{Environment.NewLine}{e.Message}");
        }
    }

    /// <summary>
    /// Gets a single Account for a User by its Id.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public async Task<Result<AbstractAccountBase?>> GetAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
            accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid ExtAcctId: `{accountId}`");

        var accountDocument = await FirstOrDefaultDocumentAsync(a =>
                a.UserId.Equals(userObjectId) &&
                a.Id.Equals(accountObjectId),
                                cancellationToken);

        if (accountDocument is null)
            return Result.Ok<AbstractAccountBase?>(null);

        var account = InfrastructureMapper.Map(accountDocument);

        if (account is null)
            throw new InvalidCastException("Failed to map AccountDocument to Account");

        return Result.Ok<AbstractAccountBase?>(account);
    }

    /// <summary>
    /// Gets a single Account for a User by its Id.
    /// </summary>
    /// <typeparam name="TType">The Type of the Account to be returned as</typeparam>
    /// <returns></returns>
    public async Task<Result<TType?>> GetAsync<TType>(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default) where TType : AbstractAccountBase
    {
        var result = await GetAsync(userId, accountId, cancellationToken);

        if (result.IsFailed)
            return Result.Fail<TType?>(result.Errors);

        if (result.Value is null)
            return Result.Ok<TType?>(default);

        return Result.Ok((TType?)result.Value);
    }

    /// <summary>
    /// Gets a IEnumerable of Accounts for a User, ordered by SortOrder and then by Name.
    /// </summary>
    public async Task<Result<IEnumerable<AbstractAccountBase>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        var docs = (await FindDocumentsAsync(a =>
            a.UserId == userObjectId, cancellationToken))?
            .OrderBy(a => a.SortOrder)
            .ThenBy(a => a.Name)
            .ToList() ?? [];

        if (!docs.Any())
            return Result.Ok(Enumerable.Empty<AbstractAccountBase>());

        var accounts = InfrastructureMapper.Map(docs);

        return Result.Ok(accounts);

        // Projection
        /*
        var matchBuilder = Builders<AccountDocument>.Filter;
        var match = matchBuilder.Eq(a => a.UserId, userId);

        var projectionBuilder = Builders<AccountDocument>.Projection;
        var projection = projectionBuilder.Exclude(a => a.AdjustmentHistories).Exclude(a => a.MonthlyTotals);

        var pipeline = new EmptyPipelineDefinition<AccountDocument>()
            .Match(match)
            .Project(projection);

        var bsonDocs = (await Collection.Aggregate().Match(match).Project(projection).ToListAsync(cancellationToken)) ?? [];

        if (!bsonDocs.Any())
            return Result.Ok(Enumerable.EmptyId<BaseAccount>());

        var docs = bsonDocs.Select(bsonDoc => BsonSerializer.Deserialize<AccountDocument>(bsonDoc));

        var accounts = InfrastructureMapper.Map(docs);

        return Result.Ok(accounts);
        */
    }

    public async Task<Result> UpdateAsync(
        AbstractAccountBase updatedAccountBase,
        CancellationToken cancellationToken = default)
    {
        var accountDoc = InfrastructureMapper.Map(updatedAccountBase);

        if (accountDoc is null)
            return Result.Fail("Could not map the Account to its Document");

        var saveCount = await UpdateDocumentAsync(accountDoc, cancellationToken);

        if (saveCount == 0)
            return Result.Fail("Could not update the Account");

        //HandleIntegrationEvents(Account);

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