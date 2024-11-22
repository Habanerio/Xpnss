using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public sealed class LineOfCreditAccount : BaseCreditAccount
{
    private LineOfCreditAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated) :
        base(
            id,
            userId,
            AccountTypes.Keys.LineOfCredit,
            accountName,
            balance,
            description,
            displayColor,
            creditLimit,
            interestRate,
            dateCreated,
            dateClosed,
            dateDeleted,
            dateUpdated)
    { }

    /// <summary>
    /// Creates an instance of a NEW Line of Credit Account.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If `id` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `userId` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `accountName` is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `creditLimit` is below 0</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `interestRate` is below 0 or above 100</exception>
    public static LineOfCreditAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated)
    {
        if (creditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        return new LineOfCreditAccount(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,

            creditLimit,
            interestRate,

            dateCreated,
            dateClosed,
            dateDeleted,
            dateUpdated);
    }

    /// <summary>
    /// Creates an instance of a NEW Line of Credit Account.
    /// </summary>
    /// <returns></returns>
    public static LineOfCreditAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,

        Money creditLimit,
        PercentageRate interestRate)
    {
        return new LineOfCreditAccount(
            AccountId.New,
            userId,
            accountName,
            Money.Zero,
            description,
            displayColor,
            creditLimit,
            interestRate,
            DateTime.UtcNow,
            null,
            null,
            null);
    }
}