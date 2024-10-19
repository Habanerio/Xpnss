using System.Collections.ObjectModel;
using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NodaTime;

namespace Habanerio.Xpnss.Modules.Accounts.Data;

/// <summary>
/// Responsible for managing the persistence of Account entities.
/// </summary>
public sealed class AccountsRepository :
    MongoDbRepository<AccountDocument>,
    IAccountsRepository
{
    public AccountsRepository(IOptions<MongoDbSettings> options) :
        base(new AccountsDbContext(options))
    { }

    public async Task<Result<AccountDocument>> AddAsync(AccountDocument account, CancellationToken cancellationToken = default)
    {
        try
        {
            account.DateCreated = DateTime.UtcNow;
            await base.AddDocumentAsync(account, cancellationToken);

            return account;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not save the Account{Environment.NewLine}{e.Message}");
        }
    }

    public async Task<Result<AccountDocument>> GetByIdAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
            accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{accountId}`");

        var doc = await FirstOrDefaultAsync(a =>
                                    a.Id.Equals(accountObjectId) && a.UserId.Equals(userId)
                                    /*&& !a.IsDeleted*/,
                                cancellationToken);

        if (doc is null)
            return Result.Fail($"Account not found for AccountId: `{accountId}` " +
                               $"and UserId: `{userId}`");

        return Result.Ok(doc);
    }

    public async Task<Result<ReadOnlyCollection<ChangeHistory>>> GetChangeHistoryAsync(
        string userId,
        string accountId,
        string timeZone,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
            accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{accountId}`");

        var doc = await FirstOrDefaultAsync(a =>
                a.Id.Equals(accountObjectId) &&
                a.UserId.Equals(userId) /*&& !a.IsDeleted*/,
            cancellationToken);

        if (doc is null)
            return Result.Fail($"Account not found for AccountId: `{accountId}` " +
                               $"and UserId: `{userId}`");

        var changes = doc.ChangeHistory.AsEnumerable();

        return Result.Ok(changes.ToList().AsReadOnly());
    }

    public async Task<Result<IEnumerable<AccountDocument>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var docs = (await FindAsync(a =>
            a.UserId == userId, cancellationToken));

        return Result.Ok(docs);
    }

    //TODO: Move to UpdateAccountDetailsCommand
    public async Task<Result<AccountDocument>> UpdateDetailsAsync(
        string userId,
        string accountId,
        string name,
        string description,
        string displayColor,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
            accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail("Invalid AccountId");

        var existingAccount = await FirstOrDefaultAsync(a =>
            a.Id == accountObjectId && a.UserId == userId,
            cancellationToken);

        if (existingAccount is null)
            return Result.Fail("Account not found");

        existingAccount.Name = name;
        existingAccount.Description = description;
        existingAccount.DisplayColor = displayColor;

        var result = await UpdateAsync(existingAccount, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(existingAccount!);
    }

    public async Task<Result> UpdateAsync(AccountDocument account, CancellationToken cancellationToken)
    {
        account.DateUpdated = DateTime.UtcNow;

        var saveCount = await UpdateDocumentAsync(account, cancellationToken);

        if (saveCount == 0)
            return Result.Fail("Could not update the account");

        return Result.Ok();
    }
}