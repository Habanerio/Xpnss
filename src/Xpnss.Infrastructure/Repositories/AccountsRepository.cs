using System.Collections.ObjectModel;
using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Domain.Accounts;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Habanerio.Xpnss.Infrastructure.Documents;
using Habanerio.Xpnss.Infrastructure.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Infrastructure.Repositories;

/// <summary>
/// Responsible for managing the persistence of Account entities.
/// </summary>
public sealed class AccountsRepository(IOptions<MongoDbSettings> options) :
    MongoDbRepository<AccountDocument>(new XpnssDbContext(options)),
    IAccountsRepository
{
    public async Task<Result<Account>> AddAsync(
        Account account,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accountDoc = Mapper.Map(account);

            if (accountDoc is null)
                return Result.Fail("Could not map the Account to its Document");

            accountDoc.DateCreated = DateTime.UtcNow;

            await AddDocumentAsync(accountDoc, cancellationToken);

            var newAccount = Mapper.Map(accountDoc);

            if (newAccount is null)
                return Result.Fail("Could not map the AccountDocument to Account");

            return newAccount;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not save the Account{Environment.NewLine}{e.Message}");
        }
    }

    public async Task<Result<Account?>> GetAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
            accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{accountId}`");

        var accountDocument = await FirstOrDefaultAsync(a =>
                                    a.Id.Equals(accountObjectId) && a.UserId.Equals(userId)
                                    /*&& !a.IsDeleted*/,
                                cancellationToken);

        if (accountDocument is null)
            return Result.Ok<Account?>(null);
        //return Result.Fail($"Account not found for AccountId: `{accountId}` " +
        //                   $"and UserId: `{userId}`");

        var account = Mapper.Map(accountDocument);

        if (account is null)
            return Result.Fail("Failed to map AccountDocument to Account");

        return Result.Ok<Account?>(account);
    }

    public async Task<Result<TType?>> GetAsync<TType>(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default) where TType : Account
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
            accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail("Invalid AccountId");

        var result = await GetAsync(userId, accountId, cancellationToken);

        if (result.IsFailed)
            return Result.Fail<TType?>(result.Errors);

        if (result.Value is null)
            return Result.Ok<TType?>(null);

        return Result.Ok<TType?>((TType)result.Value);
    }

    //public async Task<Result<ReadOnlyCollection<ChangeHistory>>> GetChangeHistoryAsync(
    //    string userId,
    //    string accountId,
    //    string timeZone,
    //    CancellationToken cancellationToken = default)
    //{
    //    if (string.IsNullOrWhiteSpace(userId))
    //        return Result.Fail("UserId cannot be null or empty");

    //    if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
    //        accountObjectId.Equals(ObjectId.New))
    //        return Result.Fail($"Invalid AccountId: `{accountId}`");

    //    var doc = await FirstOrDefaultAsync(a =>
    //            a.Id.Equals(accountObjectId) &&
    //            a.UserId.Equals(userId) /*&& !a.IsClosed*/,
    //        cancellationToken);

    //    if (doc is null)
    //        return Result.Fail($"Account not found for AccountId: `{accountId}` " +
    //                           $"and UserId: `{userId}`");

    //    var changes = doc.ChangeHistory.AsEnumerable();

    //    return Result.Ok(changes.ToList().AsReadOnly());
    //}

    public async Task<Result<IEnumerable<Account>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var docs = (await FindAsync(a =>
            a.UserId == userId, cancellationToken))?.ToList() ?? [];

        var accounts = Mapper.Map(docs);

        return Result.Ok(accounts);
    }

    public async Task<Result> UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        var accountDoc = Mapper.Map(account);

        if (accountDoc is null)
            return Result.Fail("Could not map the Account to its Document");

        accountDoc.DateUpdated = DateTime.UtcNow;

        var saveCount = await UpdateDocumentAsync(accountDoc, cancellationToken);

        if (saveCount == 0)
            return Result.Fail("Could not update the account");

        return Result.Ok();
    }
}