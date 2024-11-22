using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public sealed class CashAccount : BaseAccount
{
    private CashAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated) :
        base(
            id,
            userId,
            AccountTypes.Keys.Cash,
            accountName,
            balance,
            description,
            displayColor,
            dateCreated,
            dateClosed,
            dateDeleted,
            dateUpdated)
    { }

    public static CashAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated)
    {
        return new CashAccount(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,
            dateCreated,
            dateClosed,
            dateDeleted,
            dateUpdated);
    }

    public static CashAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor)
    {
        return new CashAccount(
            AccountId.New,
            userId,
            accountName, Money.Zero,
            description,
            displayColor,
            DateTime.UtcNow,
            null,
            null,
            null);
    }
}