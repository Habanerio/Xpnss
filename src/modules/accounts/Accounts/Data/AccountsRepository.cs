using System.Collections.ObjectModel;
using FluentResults;
using Habanerio.Core.DBs.MongoDB.EFCore;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Accounts.Data;

/// <summary>
/// Responsible for managing the persistence of Account entities.
/// </summary>
public sealed class AccountsRepository :
    MongoDbRepository<AccountDocument>,
    IAccountsRepository
{
    public AccountsRepository(IOptions<MongoDbSettings> options) : base(options)
    {
        Context = new AccountsDbContext(options);
    }

    public AccountsRepository(MongoDbContext context) : base(context) { }

    public override void Add(AccountDocument account)
    {
        base.Add(account);
    }

    [Obsolete("Currently just used to populate the data in the tests")]
    public ObjectId Add(AccountDto accountDto)
    {
        var newDoc = AccountDocument.New(
            accountDto.UserId,
            accountDto.Name,
            accountDto.AccountType,
            accountDto.Description,
            accountDto.Balance,
            accountDto.DisplayColor);

        var extendedProps = new List<KeyValuePair<string, object?>>();

        foreach (var prop in accountDto.GetType().GetProperties())
        {
            if (string.IsNullOrWhiteSpace(prop.Name) ||
                prop.Name == nameof(AccountDto.Id) ||
                prop.Name == nameof(AccountDto.UserId) ||
                prop.Name == nameof(AccountDto.Name) ||
                prop.Name == nameof(AccountDto.AccountType) ||
                prop.Name == nameof(AccountDto.Balance) ||
                prop.Name == nameof(AccountDto.Description) ||
                prop.Name == nameof(AccountDto.DisplayColor) ||
                prop.Name == nameof(AccountDto.IsCredit) ||
                prop.Name == nameof(AccountDto.IsDeleted) ||
                //prop.Name == nameof(AccountDto.ChangeHistory) ||
                prop.Name == nameof(AccountDto.DateCreated) ||
                prop.Name == nameof(AccountDto.DateUpdated) ||
                prop.Name == nameof(AccountDto.DateDeleted)
                // || prop.Name == nameof(AccountDto.ChangeHistoryItems)
                )
                continue;

            /* If/when `InterestRate` is a ValueObject
            if (prop.Name.Equals("InterestRate"))
            {
                var interestRate = prop?.GetValue(entity) as InterestRate ?? default;
                extendedProps.Add(new KeyValuePair<string, object?>(prop.Name, interestRate.Value));
                continue;
            }
            */

            var value = prop.GetValue(accountDto) ?? default;

            extendedProps.Add(new KeyValuePair<string, object?>(prop.Name, value));
        }

        newDoc.ExtendedProps = extendedProps;

        base.Add(newDoc);

        return newDoc.Id;
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
                                    a.Id.Equals(accountObjectId) &&
                                    a.UserId.Equals(userId) /*&& !a.IsDeleted*/,
                                cancellationToken);

        if (doc is null)
            return Result.Fail($"Account not found for AccountId: `{accountId}` " +
                               $"and UserId: `{userId}`");

        return Result.Ok(doc);
    }

    public async Task<Result<ReadOnlyCollection<ChangeHistory>>> GetChangeHistoryAsync(
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
                a.Id.Equals(accountObjectId) &&
                a.UserId.Equals(userId) /*&& !a.IsDeleted*/,
            cancellationToken);

        if (doc is null)
            return Result.Fail($"Account not found for AccountId: `{accountId}` " +
                               $"and UserId: `{userId}`");

        var changes = doc.ChangeHistory.AsEnumerable();

        return Result.Ok(changes.ToList().AsReadOnly());
    }

    public async Task<Result<IEnumerable<AccountDocument>>> ListByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var docs = (await FindAsync(a =>
            a.UserId == userId, cancellationToken));

        if (docs is null)
            return Result.Ok(Enumerable.Empty<AccountDocument>());

        return Result.Ok(docs);
    }

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

        var existingAccount = await DbSet.FirstOrDefaultAsync(a =>
            a.Id == accountObjectId && a.UserId == userId,
            cancellationToken);

        if (existingAccount is null)
            return Result.Fail("Account not found");

        existingAccount.Name = name;
        existingAccount.Description = description;
        existingAccount.DisplayColor = displayColor;

        var result = await SaveAsync(cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(existingAccount!);
    }

    public override void Update(AccountDocument account)
    {
        base.Update(account);
    }

    public override Task<Result> SaveAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveAsync(cancellationToken);
    }
}