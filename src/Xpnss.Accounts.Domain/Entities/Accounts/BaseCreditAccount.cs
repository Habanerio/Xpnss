using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public abstract class BaseCreditAccount : BaseAccount, IHasCreditLimit, IHasInterestRate
{
    protected BaseCreditAccount(
        UserId userId,
        AccountTypes.Keys accountType,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate) :
        base(
            userId,
            accountType,
            accountName,
            true,
            balance,
            description,
            displayColor)
    {
        if (creditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        CreditLimit = creditLimit;
        InterestRate = interestRate;
    }

    protected BaseCreditAccount(
        AccountId id,
        UserId userId,
        AccountTypes.Keys accountType,
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
            accountType,
            accountName,
            true,
            balance,
            description,
            displayColor,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        if (creditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        CreditLimit = creditLimit;
        InterestRate = interestRate;
        IsCredit = true;
    }

    public Money CreditLimit { get; set; }

    public PercentageRate InterestRate { get; set; }

    public bool IsOverLimit => Balance > CreditLimit;

    /// <summary>
    /// Applies a Transaction CreditLimit to the Account's Balance.<br />
    /// When the Transaction is a Credit, the creditLimit is SUBTRACTED from the Balance.<br />
    /// When the Transaction is a Debit, the creditLimit is ADDED to the Balance.
    /// </summary>
    /// <param name="amount">The creditLimit of the Transaction</param>
    /// <param name="transactionType">The type of Transaction that occurred</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public override void ApplyTransactionAmount(Money amount, TransactionTypes.Keys transactionType)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot add a transaction to a deleted Account");

        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "ApplyTransactionAmount creditLimit cannot be negative");

        if (!TransactionTypes.IsCreditTransaction(transactionType))
        {
            Balance += amount;
        }
        else
        {
            Balance -= amount;
        }
    }

    /// <summary>
    /// Undoes a previously applied Transaction CreditLimit from the Account's Balance (eg: for when a Transaction is deleted).<br />
    /// When the Transaction is a Credit, the creditLimit is ADDED to the Balance.<br />
    /// When the Transaction is a Debit, the creditLimit is SUBTRACTED from the Balance.
    /// </summary>
    /// <param name="amount">The creditLimit of the original Transaction</param>
    /// <param name="transactionType">The original Transaction Type</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public override void UndoTransactionAmount(Money amount, TransactionTypes.Keys transactionType)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot remove a transaction from a deleted Account");

        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "RemoveTransaction creditLimit cannot be negative");

        if (!TransactionTypes.IsCreditTransaction(transactionType))
        {
            Balance -= amount;
        }
        else
        {
            Balance += amount;
        }
    }

    ///// <summary>
    ///// With Credit Accounts, Deposits decrease the Balance (owed).
    ///// </summary>
    ///// <param name="dateOfDeposit"></param>
    ///// <param name="creditLimit"></param>
    ///// <exception cref="ArgumentOutOfRangeException"></exception>
    //public override void AddDeposit(DateTime dateOfDeposit, Money creditLimit)
    //{
    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot deposit to a deleted Account");

    //    if (creditLimit.Value < 0)
    //        throw new ArgumentOutOfRangeException(nameof(creditLimit), "AddDeposit creditLimit cannot be negative");

    //    //AddMonthlyTotalCredit(dateOfDeposit, creditLimit);

    //    // When $ is deposited to a credit account, the balance is decreased.
    //    Balance -= creditLimit;
    //}

    ///// <summary>
    ///// Removes an existing deposit value from the Account.
    ///// This is for when a mistake was made and the deposit was deleted and needs to be removed from totals.
    ///// </summary>
    ///// <param name="dateOfDeposit"></param>
    ///// <param name="creditLimit"></param>
    ///// <exception cref="InvalidOperationException"></exception>
    ///// <exception cref="ArgumentOutOfRangeException"></exception>
    //public override void RemoveDeposit(DateTime dateOfDeposit, Money creditLimit)
    //{
    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot remove a deposit from a deleted Account");

    //    if (creditLimit.Value < 0)
    //        throw new ArgumentOutOfRangeException(nameof(creditLimit), "RemoveDeposit creditLimit cannot be negative");

    //    //RemoveMonthlyTotalCredit(dateOfDeposit, creditLimit);

    //    Balance += creditLimit;
    //}

    ///// <summary>
    ///// With Credit Accounts, Withdraws increase the Balance (owed).
    ///// </summary>
    ///// <param name="dateOfWithdraw"></param>
    ///// <param name="creditLimit"></param>
    ///// <exception cref="ArgumentOutOfRangeException"></exception>
    //public override void AddWithdrawal(DateTime dateOfWithdraw, Money creditLimit)
    //{
    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot withdraw from a deleted Account");

    //    if (creditLimit.Value < 0)
    //        throw new ArgumentOutOfRangeException(nameof(creditLimit), "AddWithdrawal creditLimit cannot be negative");

    //    //AddMonthlyTotalDebit(dateOfWithdraw, creditLimit);

    //    // When $ is withdrawn from a credit account, the balance is increased.
    //    Balance += creditLimit;
    //}

    //public override void RemoveWithdrawal(DateTime dateOfWithdraw, Money creditLimit)
    //{
    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot remove a withdrawal from a deleted Account");

    //    if (creditLimit.Value < 0)
    //        throw new ArgumentOutOfRangeException(nameof(creditLimit), "RemoveWithdrawal creditLimit cannot be negative");

    //    //RemoveMonthlyTotalDebit(dateOfWithdraw, creditLimit);

    //    Balance -= creditLimit;
    //}

    /// <summary>
    /// This updates the current Credit Limit of the Account.
    /// This is for when the current Credit Limit is out of sync with reality.
    /// Use this _AFTER_ adding it to the Adjustments (Domain Event).
    /// </summary>
    /// <param name="newCreditLimit">The new value for the current Credit Limit</param>
    /// <exception cref="InvalidOperationException">When the account is marked as deleted</exception>
    /// <exception cref="ArgumentOutOfRangeException">When the new credit limit is below 0</exception>
    /// <remarks>Would prefer this to be `internal`.</remarks>
    public void UpdateCreditLimit(Money newCreditLimit)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted Account");

        if (newCreditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(newCreditLimit));

        CreditLimit = newCreditLimit;
    }

    /// <summary>
    /// This updates the current Interest Rate of the Account.
    /// This is for when the current Interest Rate is out of sync with reality.
    /// Use this _AFTER_ adding it to the Adjustments (Domain Event).
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

    //public void AddCreditLimitAdjustment(Money value, DateTime dateChanged, string reason = "")
    //{
    //    throw new NotImplementedException("Still need to think this through. AdjustmentHistory may be it's own collection. May need another service.");

    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot adjust a deleted Account");

    //    //AddAdjustmentHistory(
    //    //    nameof(CreditLimit),
    //    //    value,
    //    //    dateChanged,
    //    //    reason);

    //    //AddDomainEvent(new AccountInterestRateAdjustedEvent(Id, userId, newValue));
    //}

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