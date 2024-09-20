using FluentResults;
using Habanerio.Core.DBs.MongoDB.EFCore;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Accounts.Data;

/// <summary>
/// Responsible for managing the persistence of Account entities.
/// </summary>
public sealed class AccountsRepository : MongoDbRepository<AccountDocument>, IAccountsRepository
{
    public AccountsRepository(IOptions<MongoDbSettings> options) : base(options)
    {
        Context = new AccountsDbContext(options);
    }

    public AccountsRepository(MongoDbContext context) : base(context) { }

    public ObjectId Add(AccountDto account)
    {
        var newDoc = AccountDocument.New(
            account.UserId,
            account.Name,
            account.AccountType,
            account.Description,
            account.Balance,
            account.DisplayColor,
            account.IsCredit);

        var extendedProps = new List<KeyValuePair<string, object?>>();

        foreach (var prop in account.GetType().GetProperties())
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

            var value = prop.GetValue(account) ?? default;

            extendedProps.Add(new KeyValuePair<string, object?>(prop.Name, value));
        }

        newDoc.ExtendedProps = extendedProps;

        base.Add(newDoc);

        return newDoc.Id;
    }

    public async Task<Result<AccountDto>> GetByIdAsync(string accountId, string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(accountId, out var accountObjectId) || accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{accountId}`");

        var doc = await FirstOrDefaultAsync(a =>
                                    a.Id.Equals(accountObjectId) &&
                                    a.UserId.Equals(userId) /*&& !a.IsDeleted*/,
                                cancellationToken);

        if (doc is null)
            return Result.Fail($"Account not found for AccountId: `{accountId}` and UserId: `{userId}`");

        var dto = Mappers.DocumentToDtoMappings.Map(doc);

        if (dto is null)
            return Result.Fail("Failed to map AccountDocument to AccountDto");

        return Result.Ok(dto);
    }

    public async Task<Result<IEnumerable<AccountDto>>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var docs = await FindAsync(a => a.UserId == userId, cancellationToken);

        var dtos = Mappers.DocumentToDtoMappings.Map(docs);

        return Result.Ok(dtos);
    }

    public override Task<Result> SaveAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveAsync(cancellationToken);
    }
}