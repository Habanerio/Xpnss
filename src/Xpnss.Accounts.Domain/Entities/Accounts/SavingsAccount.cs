using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public sealed class SavingsAccount : BaseAccount, IHasInterestRate
{
    public PercentageRate InterestRate { get; set; }

    /// <summary>
    /// Actual creation of a NEW or EXISTING instance of a Savings Account.
    /// </summary>
    private SavingsAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        PercentageRate interestRate,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated) :
        base(
            id,
            userId,
            AccountTypes.Keys.Savings,
            accountName,
            balance,
            description,
            displayColor,
            dateCreated,
            dateClosed,
            dateDeleted,
            dateUpdated)
    {
        InterestRate = interestRate;
    }

    /// <summary>
    /// Creates an instance of an EXISTING Savings Account.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If `id` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `userId` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `accountName` is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `interestRate` is below 0 or above 100</exception>
    public static SavingsAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        PercentageRate interestRate,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated)
    {
        return new SavingsAccount(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,
            interestRate,
            dateCreated,
            dateClosed,
            dateDeleted,
            dateUpdated);
    }

    /// <summary>
    /// Creates an instance of a NEW Savings Account.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If `userId` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `accountName` is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `interestRate` is below 0 or above 100</exception>
    public static SavingsAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        PercentageRate interestRate)
    {
        return new SavingsAccount(
            AccountId.New,
            userId,
            accountName,
            Money.Zero,
            description,
            displayColor,
            interestRate,
            DateTime.UtcNow,
            null,
            null,
            null);
    }

    /// <summary>
    /// This updates the current Interest Rate of the Account.
    /// This is for when the current Interest Rate is out of sync with reality.
    /// Use this AFTER adding it to the Adjustments.
    /// </summary>
    /// <param name="newInterestRate">The new value for the current Interest Rate</param>
    /// <exception cref="InvalidOperationException">When the account is marked as deleted</exception>
    /// <remarks>Would prefer this to be `internal`.</remarks>
    public void UpdateInterestRate(PercentageRate newInterestRate)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted Account");

        InterestRate = newInterestRate;
    }

    //public void AddInterestRateAdjustment(PercentageRate value, DateTime dateChanged, string reason = "")
    //{
    //    throw new NotImplementedException("Still need to think this through. AdjustmentHistory may be it's own collection. May need another service.");

    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot adjust a deleted Account");

    //    //AddAdjustmentHistory(
    //    //    nameof(InterestRate),
    //    //    value,
    //    //    dateChanged,
    //    //    reason);

    //    //AddDomainEvent(new AccountInterestRateAdjustedEvent(Id, userId, newValue));
    //}
}