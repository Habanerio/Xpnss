using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

/// <summary>
/// For any accounts that were imported and if we don't know what type they are,
/// or can't match with an existing account type.
/// </summary>
public class UnknownImportedAccount : BaseAccount
{
    private UnknownImportedAccount(
        UserId userId,
        AccountName accountName,
        bool isCredit,
        Money balance,
        string description) :
        base(
            userId,
            AccountTypes.Keys.UNKNOWN,
            accountName,
            isCredit,
            balance,
            description,
            "#ff0000")
    { }

    private UnknownImportedAccount(
        AccountId accountId,
        UserId userId,
        AccountName accountName,
        bool isCredit,
        Money balance,
        string description,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(accountId,
            userId,
            AccountTypes.Keys.UNKNOWN,
            accountName,
            isCredit,
            balance,
            description,
            "#ff0000",
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }

    public static UnknownImportedAccount Load(
        AccountId accountId,
        UserId userId,
        AccountName accountName,
        bool isCredit,
        Money balance,
        string description,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null)
    {
        return new(
            accountId,
            userId,
            accountName,
            isCredit,
            balance,
            description,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }


    public static UnknownImportedAccount New(
        UserId userId,
        AccountName accountName,
        bool isCredit,
        Money balance,
        string description)
    {
        return new(userId, accountName, isCredit, balance, description);
    }
}