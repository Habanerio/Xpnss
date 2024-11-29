using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public abstract class BaseAccount : AggregateRoot<AccountId>
{
    public UserId UserId { get; }

    public AccountTypes.Keys AccountType { get; private set; }

    /// <summary>
    /// Name of the specific Account type
    /// </summary>
    /// <example>Capital One (Credit Card)</example>
    public AccountName Name { get; private set; }

    public Money Balance { get; protected set; }

    public string Description { get; private set; }


    public string DisplayColor { get; private set; }

    public bool IsClosed => DateClosed.HasValue;

    public bool IsCredit { get; set; }

    public DateTime? DateClosed { get; protected set; }

    // New Accounts
    // Adds a Domain Event for the creation of a new Account
    protected BaseAccount(
        UserId userId,
        AccountTypes.Keys accountType,
        AccountName accountName,
        bool isCredit,
        Money balance,
        string description,
        string displayColor) :
        this(
            AccountId.Empty,
            userId,
            accountType,
            accountName,
            isCredit,
            balance,
            description,
            displayColor,
            null,
            DateTime.UtcNow)
    {
        IsCredit = isCredit;
        IsTransient = true;

        // Add `AccountCreated` Domain Event
    }

    protected BaseAccount(
        AccountId accountId,
        UserId userId,
        AccountTypes.Keys accountType,
        AccountName accountName,
        bool isCredit,
        Money balance,
        string description,
        string displayColor,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) : base(accountId)
    {
        Id = accountId ?? throw new ArgumentNullException(nameof(accountId));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        AccountType = accountType;
        Name = accountName;
        Balance = new Money(balance);
        IsCredit = isCredit;
        DisplayColor = displayColor?.Trim() ?? string.Empty;
        Description = description?.Trim() ?? string.Empty;
        DateCreated = dateCreated;
        DateClosed = dateClosed;
        DateDeleted = dateDeleted;
        DateUpdated = dateUpdated;
    }

    /// <summary>
    /// Applies a Transaction CreditLimit to the Account's Balance.<br />
    /// When the Transaction is a Credit, the creditLimit is added to the Balance.<br />
    /// When the Transaction is a Debit, the creditLimit is subtracted from the Balance.
    /// </summary>
    /// <param name="amount">The creditLimit of the Transaction</param>
    /// <param name="transactionType">The type of Transaction that occurred</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual void ApplyTransactionAmount(Money amount, TransactionTypes.Keys transactionType)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot add a transaction to a deleted Account");

        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "ApplyTransactionAmount creditLimit cannot be negative");

        if (TransactionTypes.IsCreditTransaction(transactionType))
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
    /// When the Transaction is a Credit, the creditLimit will be SUBTRACTED from the Balance.<br />
    /// When the Transaction is a Debit, the creditLimit will be ADDED to the Balance.
    /// </summary>
    /// <param name="amount">The creditLimit of the original Transaction</param>
    /// <param name="transactionType">The original Transaction Type</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual void UndoTransactionAmount(Money amount, TransactionTypes.Keys transactionType)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot remove a transaction from a deleted Account");

        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "RemoveTransaction creditLimit cannot be negative");

        if (TransactionTypes.IsCreditTransaction(transactionType))
        {
            Balance -= amount;
        }
        else
        {
            Balance += amount;
        }
    }

    //public virtual void AddDeposit(DateTime dateOfDeposit, Money creditLimit)
    //{
    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot deposit to a deleted Account");

    //    if (creditLimit.Value < 0)
    //        throw new ArgumentOutOfRangeException(nameof(creditLimit), "AddDeposit creditLimit cannot be negative");

    //    //AddMonthlyTotalCredit(dateOfDeposit, creditLimit);

    //    Balance += creditLimit;

    //    //UpdateBalanceFromHistory();
    //}

    //public virtual void RemoveDeposit(DateTime dateOfDeposit, Money creditLimit)
    //{
    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot remove a deposit from a deleted Account");

    //    if (creditLimit.Value < 0)
    //        throw new ArgumentOutOfRangeException(nameof(creditLimit), "RemoveDeposit creditLimit cannot be negative");

    //    //RemoveMonthlyTotalCredit(dateOfDeposit, creditLimit);

    //    Balance -= creditLimit;

    //    //UpdateBalanceFromHistory();
    //}


    //public virtual void AddWithdrawal(DateTime dateOfWithdraw, Money creditLimit)
    //{
    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot withdraw from a deleted Account");

    //    if (creditLimit.Value < 0)
    //        throw new ArgumentOutOfRangeException(nameof(creditLimit), "AddWithdrawal creditLimit cannot be negative");

    //    //AddMonthlyTotalDebit(dateOfWithdraw, creditLimit);

    //    Balance -= creditLimit;

    //    //UpdateBalanceFromHistory();
    //}

    //public virtual void RemoveWithdrawal(DateTime dateOfWithdraw, Money creditLimit)
    //{
    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot remove a withdrawal from a deleted Account");

    //    if (creditLimit.Value < 0)
    //        throw new ArgumentOutOfRangeException(nameof(creditLimit), "RemoveWithdrawal creditLimit cannot be negative");

    //    //RemoveMonthlyTotalDebit(dateOfWithdraw, creditLimit);

    //    Balance += creditLimit;

    //    //UpdateBalanceFromHistory();
    //}

    /// <summary>
    /// This updates the current balance of the Account.
    /// This is for when the current balance is out of sync with reality.
    /// Use this _AFTER_ adding it to the Adjustments (Domain Event).
    /// </summary>
    /// <param name="newBalance">The new value for the current balance</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>Would prefer this to be `internal`.</remarks>
    public void UpdateBalance(Money newBalance)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update the balance of a deleted Account");

        Balance = newBalance;
    }


    ///// <summary>
    ///// Adjusts the balance of the Account.
    ///// This is for when the balance is out of sync with reality.
    ///// It is NOT used for when a transaction is added or removed.
    ///// </summary>
    ///// <param name="value">The new value of the Balance to be updated for the provided date</param>
    ///// <param name="dateChanged">The actual date that the adjustment was made. Can be in the past</param>
    ///// <param name="reason">The user's reason why the adjustment was made</param>
    //public void AddBalanceAdjustment(Money value, DateTime dateChanged, string reason = "")
    //{
    //    throw new NotImplementedException("Still need to think this through. AdjustmentHistory may be it's own collection. May need another service.");

    //    if (IsDeleted)
    //        throw new InvalidOperationException("Cannot adjust a deleted Account");

    //    AddAdjustmentHistory(
    //        nameof(Balance),
    //        value,
    //        dateChanged,
    //        reason);

    //    //AddDomainEvent(new AccountBalanceAdjustedEvent(Id, userId, newValue));
    //}

    public void UpdateDetails(string accountName, string description, string displayColour)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted Account");

        Name = !string.IsNullOrWhiteSpace(accountName) ? new AccountName(accountName) : Name;
        Description = description;
        DisplayColor = displayColour;

        DateUpdated = DateTime.UtcNow;
    }

    public void Close(DateTime dateClosed)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot close a deleted Account");

        if (IsClosed)
            return;

        DateClosed = dateClosed;//.ToUniversalTime();
        DateUpdated = DateTime.UtcNow;

        //AddDomainEvent(new AccountClosedDomainEvent(Id, userId, dateClosed));
    }

    public void ReOpen()
    {
        if (!IsClosed)
            return;

        DateClosed = null;
        DateUpdated = DateTime.UtcNow;

        //AddDomainEvent(new AccountReOpenedDomainEvent(Id, userId));
    }

    public void Delete()
    {
        if (IsDeleted)
            return;

        var now = DateTime.UtcNow;
        DateDeleted = now;
        DateUpdated = now;

        //AddDomainEvent(new AccountDeletedDomainEvent(Id, UserId, now));
    }

    /*
    internal void AddAdjustmentHistory(string property, Money value, DateTime dateChanged, string reason)
    {
        var accountId = Id;

        var adjustment = AdjustmentHistory.NewId(
            accountId,
            UserId,
            property,
            value.Value.ToString(CultureInfo.InvariantCulture),
            dateChanged.ToUniversalTime().Date,
            reason);

        // If there is already an adjustment for this property on this date, remove it
        var preExistingAdjustment = _adjustmentHistories.Find(x => x.Property == property && x.DateChanged == dateChanged.ToUniversalTime().Date);

        if (preExistingAdjustment != null)
            _adjustmentHistories.RemoveDocument(preExistingAdjustment);

        _adjustmentHistories.Add(adjustment);
    }

    internal void AddAdjustmentHistory(string property, PercentageRate value, DateTime dateChanged, string reason)
    {
        var accountId = Id;

        var adjustment = AdjustmentHistory.NewId(
            accountId,
            UserId,
            property,
            value.Value.ToString(CultureInfo.InvariantCulture),
            dateChanged.ToUniversalTime().Date,
            reason);

        // If there is already an adjustment for this property on this date, remove it
        var preExistingAdjustment = _adjustmentHistories.Find(x => x.Property == property && x.DateChanged == dateChanged.ToUniversalTime().Date);

        if (preExistingAdjustment != null)
            _adjustmentHistories.RemoveDocument(preExistingAdjustment);

        _adjustmentHistories.Add(adjustment);
    }


    public void LoadAdjustmentHistories(List<AdjustmentHistory> changeHistories)
    {
        _adjustmentHistories = changeHistories;
    }


    /// <summary>
    /// Loads an existing collection of monthly totals into the Account.
    /// </summary>
    /// <param name="monthlyTotals"></param>
    public void LoadMonthlyTotals(IEnumerable<AccountMonthlyTotal> monthlyTotals)
    {
        _monthlyTotals = monthlyTotals?.ToList() ?? [];
    }
    */

    ///// <summary>
    ///// Adds a Credit creditLimit to the Monthly Totals.
    ///// </summary>
    ///// <param name="dateOfCredit">The date that the credit was made</param>
    ///// <param name="creditLimit">The creditLimit of the credit</param>
    //internal void AddMonthlyTotalCredit(DateTime dateOfCredit, Money creditLimit)
    //{
    //    var monthlyTotal = _monthlyTotals
    //        .Find(x =>
    //            x.Year == dateOfCredit.ToUniversalTime().Year &&
    //            x.Month == dateOfCredit.ToUniversalTime().Month);

    //    if (monthlyTotal == null)
    //    {
    //        var accountId = Id;

    //        monthlyTotal = AccountMonthlyTotal.NewId(
    //            accountId,
    //            UserId,
    //            dateOfCredit.Year,
    //            dateOfCredit.Month,
    //            creditTotalAmount: creditLimit,
    //            creditCount: 1,
    //            debitTotalAmount: Money.Zero,
    //            debitCount: 0
    //        );

    //        _monthlyTotals.Add(monthlyTotal);
    //    }
    //    else
    //    {
    //        monthlyTotal.CreditTotalAmount += creditLimit;
    //        monthlyTotal.CreditCount++;
    //    }
    //}

    //internal void RemoveMonthlyTotalCredit(DateTime dateOfCredit, Money creditLimit)
    //{
    //    var monthlyTotal = _monthlyTotals.Find(x => x.Year == dateOfCredit.Year && x.Month == dateOfCredit.Month);

    //    if (monthlyTotal == null)
    //        return;

    //    monthlyTotal.CreditTotalAmount -= creditLimit;
    //    monthlyTotal.CreditCount--;
    //}

    ///// <summary>
    ///// Adds a Debit creditLimit to the Monthly Totals.
    ///// </summary>
    ///// <param name="dateOfDebit">The date that the debit was made</param>
    ///// <param name="creditLimit">The creditLimit of the debit</param>
    //internal void AddMonthlyTotalDebit(DateTime dateOfDebit, Money creditLimit)
    //{
    //    var monthlyTotal = _monthlyTotals.Find(x => x.Year == dateOfDebit.Year && x.Month == dateOfDebit.Month);

    //    if (monthlyTotal == null)
    //    {
    //        var accountId = Id;

    //        monthlyTotal = AccountMonthlyTotal.NewId(
    //            accountId,
    //            UserId,
    //            dateOfDebit.Year,
    //            dateOfDebit.Month,
    //            creditTotalAmount: Money.Zero,
    //            creditCount: 0,
    //            debitTotalAmount: creditLimit,
    //            debitCount: 1
    //        );

    //        _monthlyTotals.Add(monthlyTotal);
    //    }
    //    else
    //    {
    //        monthlyTotal.DebitTotalAmount += creditLimit;
    //        monthlyTotal.DebitCount++;
    //    }
    //}

    //internal void RemoveMonthlyTotalDebit(DateTime dateOfDebit, Money creditLimit)
    //{
    //    var monthlyTotal = _monthlyTotals.Find(x => x.Year == dateOfDebit.Year && x.Month == dateOfDebit.Month);

    //    if (monthlyTotal == null)
    //        return;

    //    monthlyTotal.DebitTotalAmount -= creditLimit;
    //    monthlyTotal.DebitCount--;
    //}

    //internal void UpdateBalanceFromAdjustmentHistories()
    //{
    //    //var deposits = MonthlyDepositTotals.Sum(x => x.Total);
    //    //var withdraws = MonthlyWithdrawTotals.Sum(x => x.Total);

    //    //var balanceHistories = AdjustmentHistories.FindAll(x => x.Property == "Balance");

    //    //Balance = deposits - withdraws;
    //}
}
