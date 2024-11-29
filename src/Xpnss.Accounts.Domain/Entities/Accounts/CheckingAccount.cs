using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public sealed class CheckingAccount : BaseAccount, IHasOverdraftAmount
{
    public Money OverdraftAmount { get; private set; }

    public bool IsOverLimit => Balance.Value < -1 * OverdraftAmount.Value;

    private CheckingAccount(
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money overDraftAmount) :
        base(
            userId,
            AccountTypes.Keys.Checking,
            accountName,
            false,
            balance,
            description,
            displayColor)
    {
        OverdraftAmount = overDraftAmount;
    }

    private CheckingAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money overDraftAmount,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            AccountTypes.Keys.Checking,
            accountName,
            false,
            balance,
            description,
            displayColor,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        OverdraftAmount = overDraftAmount;
    }

    /// <summary>
    /// Creates an instance of an EXISTING Checking Account.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If `id` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `userId` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `accountName` is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `overDraftAmount` is below 0</exception>
    public static CheckingAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money overDraftAmount,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateDeleted,
        DateTime? dateUpdated)
    {
        if (overDraftAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(overDraftAmount));

        return new CheckingAccount(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,
            overDraftAmount,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    /// <summary>
    /// Creates an instance of a NEW Checking Account.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If `userId` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `accountName` is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `overDraftAmount` is below 0</exception>
    public static CheckingAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        Money overDraftAmount)
    {
        if (overDraftAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(overDraftAmount));

        return new CheckingAccount(
            userId,
            accountName,
            Money.Zero,
            description,
            displayColor,
            overDraftAmount);
    }

    /// <summary>
    /// This updates the current Overdraft CreditLimit of the Account.
    /// This is for when the current Overdraft CreditLimit is out of sync with reality.
    /// Use this _AFTER_ adding it to the Adjustments (Domain Event).
    /// </summary>
    /// <param name="newOverdraftAmount">The new value for the current OverdraftA mount</param>
    /// <exception cref="InvalidOperationException">When the account is marked as deleted</exception>
    /// <exception cref="ArgumentOutOfRangeException">When the new Overdraft CreditLimit is below 0</exception>
    /// <remarks>Would prefer this to be `internal`.</remarks>
    public void UpdateOverdraftAmount(Money newOverdraftAmount)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted Account");

        if (newOverdraftAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(newOverdraftAmount));

        OverdraftAmount = newOverdraftAmount;
    }

    //public void AddOverdraftAdjustment(Money value, DateTime dateChanged, string reason = "")
    //{
    //    throw new NotImplementedException("Still need to think this through. AdjustmentHistory may be it's own collection. May need another service.");

    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot adjust a deleted Account");

    //    //AddAdjustmentHistory(
    //    //    nameof(OverdraftAmount),
    //    //    value,
    //    //    dateChanged, reason);

    //    //AddDomainEvent(new AccountOverdraftAmountAdjustedEvent(Id, userId, newValue));
    //}
}