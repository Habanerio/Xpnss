using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public sealed class CashAccount : BaseAccount
{
    // New Cash Accounts
    private CashAccount(
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor) :
        base(
            userId,
            AccountTypes.Keys.Cash,
            accountName,
            false,
            balance,
            description,
            displayColor)
    { }

    // Existing Cash Accounts
    private CashAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            AccountTypes.Keys.Cash,
            accountName,
            false,
            balance,
            description,
            displayColor,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }

    public static CashAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new CashAccount(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    public static CashAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor)
    {
        return new CashAccount(
            userId,
            accountName, Money.Zero,
            description,
            displayColor);
    }
}