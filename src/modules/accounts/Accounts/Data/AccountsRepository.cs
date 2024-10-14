using System.Collections.ObjectModel;
using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

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


    //[Obsolete("Currently just used to populate the data in the tests")]
    //public Result<ObjectId> Add(AccountDto accountDto)
    //{
    //    var newDoc = AccountDocument.New(
    //        accountDto.UserId,
    //        accountDto.Name,
    //        accountDto.AccountTypes,
    //        accountDto.Description,
    //        accountDto.Balance,
    //        accountDto.DisplayColor);

    //    if (accountDto.AccountTypes == AccountTypes.Cash)
    //    {
    //        if (accountDto is not CashAccountDto cashDto)
    //            return Result.Fail("Invalid CashAccountDto");

    //        var cash = newDoc as CashAccount;

    //        base.AddDocument(cash);
    //    }
    //    else if (accountDto.AccountTypes == AccountTypes.Checking)
    //    {
    //        if (accountDto is not CheckingAccountDto checkingDto)
    //            return Result.Fail("Invalid CheckingAccountDto");

    //        var checking = newDoc as CheckingAccount;

    //        checking.OverDraftAmount = checkingDto.OverDraftAmount;

    //        base.AddDocument(checking);
    //    }

    //    else if (accountDto.AccountTypes == AccountTypes.Savings)
    //    {
    //        newDoc = (SavingsAccount)newDoc;

    //        if (accountDto is not SavingsAccountDto savingsDto)
    //            return Result.Fail("Invalid SavingsAccountDto");

    //        var savings = newDoc as SavingsAccount;
    //        savings.InterestRate = savingsDto.InterestRate;

    //        base.AddDocument(savings);
    //    }
    //    else if (accountDto.AccountTypes == AccountTypes.CreditCard)
    //    {
    //        if (accountDto is not CreditCardAccountDto creditCardDto)
    //            return Result.Fail("Invalid CreditCardAccountDto");

    //        var creditCard = newDoc as CreditCardAccount;
    //        creditCard.CreditLimit = creditCardDto.CreditLimit;
    //        creditCard.InterestRate = creditCardDto.InterestRate;

    //        base.AddDocument(creditCard);
    //    }
    //    else if (accountDto.AccountTypes == AccountTypes.LineOfCredit)
    //    {
    //        if (accountDto is not LineOfCreditAccountDto locDto)
    //            return Result.Fail("Invalid LineOfCreditAccountDto");

    //        var loc = newDoc as LineOfCreditAccount;
    //        loc.CreditLimit = locDto.CreditLimit;
    //        loc.InterestRate = locDto.InterestRate;

    //        base.AddDocument(loc);
    //    }
    //    else
    //    {
    //        return Result.Fail($"Invalid AccountTypes: {accountDto.AccountTypes}");
    //    }

    //    //var extendedProps = new List<KeyValuePair<string, object?>>();

    //    //foreach (var prop in accountDto.GetType().GetProperties())
    //    //{
    //    //    if (string.IsNullOrWhiteSpace(prop.Name) ||
    //    //        prop.Name == nameof(AccountDto.Id) ||
    //    //        prop.Name == nameof(AccountDto.UserId) ||
    //    //        prop.Name == nameof(AccountDto.Name) ||
    //    //        prop.Name == nameof(AccountDto.AccountTypes) ||
    //    //        prop.Name == nameof(AccountDto.Balance) ||
    //    //        prop.Name == nameof(AccountDto.Description) ||
    //    //        prop.Name == nameof(AccountDto.DisplayColor) ||
    //    //        prop.Name == nameof(AccountDto.IsCredit) ||
    //    //        prop.Name == nameof(AccountDto.IsDeleted) ||
    //    //        //prop.Name == nameof(AccountDto.ChangeHistory) ||
    //    //        prop.Name == nameof(AccountDto.DateCreated) ||
    //    //        prop.Name == nameof(AccountDto.DateUpdated) ||
    //    //        prop.Name == nameof(AccountDto.DateDeleted)
    //    //        // || prop.Name == nameof(AccountDto.ChangeHistoryItems)
    //    //        )
    //    //        continue;

    //    //    /* If/when `InterestRate` is a ValueObject
    //    //    if (prop.Name.Equals("InterestRate"))
    //    //    {
    //    //        var interestRate = prop?.GetValue(entity) as InterestRate ?? default;
    //    //        extendedProps.AddDocument(new KeyValuePair<string, object?>(prop.Name, interestRate.Value));
    //    //        continue;
    //    //    }
    //    //    */

    //    //    var value = prop.GetValue(accountDto) ?? default;

    //    //    extendedProps.Add(new KeyValuePair<string, object?>(prop.Name, value));
    //    //}

    //    //newDoc.ExtendedProps = extendedProps;

    //    //base.AddDocument(newDoc);

    //    return newDoc.Id;
    //}

    public async Task<Result<ObjectId>> AddAsync(AccountDocument account, CancellationToken cancellationToken = default)
    {
        try
        {
            await base.AddDocumentAsync(account, cancellationToken);

            return account.Id;
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

        //var filter = Builders<AccountDocument>.Filter.Eq(a => a.UserId, userId);

        //var docs = await FindAsync(filter, cancellationToken);

        var docs = (await FindAsync(a =>
            a.UserId == userId, cancellationToken));

        if (docs is null)
            return Result.Ok(Enumerable.Empty<AccountDocument>());

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
        var saveCount = await UpdateDocumentAsync(account, cancellationToken);

        if (saveCount == 0)
            return Result.Fail("Could not update the account");

        return Result.Ok();
    }
}