using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public sealed class CreditCardAccount : BaseCreditAccount
{
    private CreditCardAccount(
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate) :
        base(
            userId,
            AccountTypes.Keys.CREDIT_CARD,
            accountName,
            balance,
            description,
            displayColor,
            creditLimit,
            interestRate)
    { }

    private CreditCardAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            AccountTypes.Keys.CREDIT_CARD,
            accountName,
            balance,
            description,
            displayColor,
            creditLimit,
            interestRate,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }

    /// <summary>
    /// Creates an instance of a NEW Credit Card Account.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If `id` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `userId` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `accountName` is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `creditLimit` is below 0</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `interestRate` is below 0 or above 100</exception>
    public static CreditCardAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new CreditCardAccount(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,
            creditLimit,
            interestRate,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    /// <summary>
    /// Creates an instance of a NEW Credit Card Account.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If `userId` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `accountName` is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `creditLimit` is below 0</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `interestRate` is below 0 or above 100</exception>
    public static CreditCardAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,

        Money creditLimit,
        PercentageRate interestRate)
    {
        return new CreditCardAccount(
            userId,
            accountName,
            Money.Zero,
            description,
            displayColor,
            creditLimit,
            interestRate);
    }
}