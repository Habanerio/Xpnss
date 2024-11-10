using System.Globalization;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Habanerio.Xpnss.Domain.ChangeHistories;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Accounts;

public abstract class Account : AggregateRoot<AccountId>
{
    private List<ChangeHistory> _changeHistory = [];
    private List<MonthlyAccountTotals> _monthlyTotals = [];

    public UserId UserId { get; }

    public AccountTypes.Keys AccountType { get; init; }

    /// <summary>
    /// Name of the specific account type
    /// </summary>
    /// <example>Capital One (Credit Card)</example>
    public AccountName Name { get; private set; }

    public Money Balance { get; protected set; }

    public IReadOnlyCollection<ChangeHistory> ChangeHistory => _changeHistory.AsReadOnly();

    public string Description { get; set; }


    public string DisplayColor { get; set; }

    public bool IsClosed => DateClosed.HasValue;

    public bool IsCredit { get; set; }

    public IReadOnlyCollection<MonthlyAccountTotals> MonthlyTotals => _monthlyTotals.AsReadOnly();

    public DateTime DateCreated { get; init; }

    public DateTime? DateUpdated { get; protected set; }

    public DateTime? DateClosed { get; init; }

    public DateTime? DateDeleted { get; init; }

    protected Account(
        AccountId accountId,
        UserId userId,
        AccountTypes.Keys accountType,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated) : base(accountId)
    {
        Id = accountId ?? throw new ArgumentNullException(nameof(accountId));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        AccountType = accountType;
        Name = accountName;
        Balance = new Money(balance);
        DisplayColor = displayColor?.Trim() ?? string.Empty;
        Description = description?.Trim() ?? string.Empty;
        DateCreated = dateCreated;
        DateClosed = dateClosed;
        DateDeleted = dateDeleted;
        DateUpdated = dateUpdated;
    }

    public virtual void Deposit(DateTime dateOfDeposit, Money amount)
    {
        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount cannot be negative");

        AddMonthlyTotalCredit(dateOfDeposit, amount);

        Balance += amount;

        //UpdateBalanceFromHistory();
    }

    public virtual void Withdraw(DateTime dateOfWithdraw, Money amount)
    {
        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Withdraw amount cannot be negative");

        AddMonthlyTotalDebit(dateOfWithdraw, amount);

        Balance -= amount;

        //UpdateBalanceFromHistory();
    }


    ///// <summary>
    ///// Removes an amount from one or the other Monthly Totals.
    ///// This is used when a transaction is deleted.
    ///// </summary>
    ///// <param name="isDeposit"></param>
    ///// <param name="date"></param>
    ///// <param name="amount"></param>
    //public void RemoveFromMonthlyTotals(bool isDeposit, DateTime date, decimal amount)
    //{
    //    var monthlyTotals = isDeposit ? MonthlyDepositTotals : MonthlyWithdrawTotals;

    //    var monthlyTotal = monthlyTotals.Find(x => x.Year == date.Year && x.Month == date.Month);

    //    if (monthlyTotal != null)
    //    {
    //        monthlyTotal.Total += amount;
    //        monthlyTotal.TransactionCount--;

    //        UpdateBalanceFromHistory();
    //    }
    //}

    //public void UpdateBalace(TransactionTypes.Keys transactionType, Money amount, DateTime dateOfTransaction)
    //{

    //}

    /// <summary>
    /// Adjusts the balance of the account.
    /// This is for when the balance is out of sync with reality.
    /// It is NOT used for when a transaction is added or removed.
    /// </summary>
    /// <param name="newValue">The new value to be updated to</param>
    /// <param name="dateChanged">The actual date that the adjustment was made. Can be in the past</param>
    /// <param name="reason">The user's reason why the adjustment was made</param>
    public void AdjustBalance(Money newValue, DateTime dateChanged, string reason = "")
    {
        var previousValue = Balance;
        Balance = newValue;

        DateUpdated = DateTime.UtcNow;

        AddChangeHistory(
            nameof(Balance),
            previousValue,
            newValue,
            dateChanged,
            reason);

        //AddDomainEvent(new AccountBalanceAdjustedEvent(Id, userId, newValue));
    }

    public void UpdateDetails(string accountName, string description, string displayColour)
    {
        Name = !string.IsNullOrWhiteSpace(accountName) ? new AccountName(accountName) : Name;
        Description = description;
        DisplayColor = displayColour;

        DateUpdated = DateTime.UtcNow;
    }

    internal void AddChangeHistory(string property, Money oldValue, Money newValue, DateTime dateChanged, string reason)
    {
        _changeHistory.Add(
            new ChangeHistory(
                Id,
                UserId,
                property,
                oldValue.Value.ToString(CultureInfo.InvariantCulture),
                newValue.Value.ToString(CultureInfo.InvariantCulture),
                dateChanged.ToUniversalTime(),
                reason)
            );
    }

    internal void AddChangeHistory(string property, PercentageRate oldValue, PercentageRate newValue, DateTime dateChanged, string reason)
    {
        _changeHistory.Add(
            new ChangeHistory(
                Id,
                UserId,
                property,
                oldValue.Value.ToString(CultureInfo.InvariantCulture),
                newValue.Value.ToString(CultureInfo.InvariantCulture),
                dateChanged.ToUniversalTime(),
                reason)
        );
    }

    public void LoadChangeHistories(List<ChangeHistory> changeHistories)
    {
        _changeHistory = changeHistories;
    }


    /// <summary>
    /// Loads an existing collection of monthly totals into the Account.
    /// </summary>
    /// <param name="monthlyTotals"></param>
    public void LoadMonthlyTotals(List<MonthlyAccountTotals> monthlyTotals)
    {
        _monthlyTotals = monthlyTotals;
    }

    /// <summary>
    /// Adds a Credit amount to the Monthly Totals.
    /// </summary>
    /// <param name="dateOfCredit">The date that the credit was made</param>
    /// <param name="amount">The amount of the credit</param>
    internal void AddMonthlyTotalCredit(DateTime dateOfCredit, Money amount)
    {
        var monthlyTotal = _monthlyTotals
            .Find(x =>
                x.Year == dateOfCredit.ToUniversalTime().Year &&
                x.Month == dateOfCredit.ToUniversalTime().Month);

        if (monthlyTotal == null)
        {
            monthlyTotal = new MonthlyAccountTotals(
                Id,
                UserId,
                dateOfCredit.Year,
                dateOfCredit.Month,

                creditTotalAmount: amount,
                creditCount: 1,

                debitTotalAmount: Money.Zero,
                debitCount: 0
            );

            _monthlyTotals.Add(monthlyTotal);
        }
        else
        {
            monthlyTotal.CreditTotalAmount += amount;
            monthlyTotal.CreditCount++;
        }
    }

    /// <summary>
    /// Adds a Debit amount to the Monthly Totals.
    /// </summary>
    /// <param name="dateOfDebit">The date that the debit was made</param>
    /// <param name="amount">The amount of the debit</param>
    internal void AddMonthlyTotalDebit(DateTime dateOfDebit, Money amount)
    {
        var monthlyTotal = _monthlyTotals.Find(x => x.Year == dateOfDebit.Year && x.Month == dateOfDebit.Month);

        if (monthlyTotal == null)
        {
            monthlyTotal = new MonthlyAccountTotals(

                Id,
                UserId,
                dateOfDebit.Year,
                dateOfDebit.Month,

                creditTotalAmount: Money.Zero,
                creditCount: 0,

                debitTotalAmount: amount,
                debitCount: 1
            );

            _monthlyTotals.Add(monthlyTotal);
        }
        else
        {
            monthlyTotal.DebitTotalAmount += amount;
            monthlyTotal.DebitCount++;
        }
    }

    internal void UpdateBalanceFromHistory()
    {
        //var deposits = MonthlyDepositTotals.Sum(x => x.Total);
        //var withdraws = MonthlyWithdrawTotals.Sum(x => x.Total);

        //var balanceHistories = ChangeHistory.FindAll(x => x.Property == "Balance");

        //Balance = deposits - withdraws;
    }
}

public sealed class CashAccount : Account
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

public sealed class CheckingAccount : Account, IHasOverDraftAmount
{
    public Money OverDraftAmount { get; private set; }

    public bool IsOverLimit => Balance.Value < (-1 * OverDraftAmount.Value);

    private CheckingAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money overDraftAmount,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated) :
        base(
            id,
            userId,
            AccountTypes.Keys.Checking,
            accountName,
            balance,
            description,
            displayColor,
            dateCreated,
            dateClosed,
            dateDeleted,
            dateUpdated)
    {
        OverDraftAmount = overDraftAmount;
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
        DateTime dateCreated,
        DateTime? dateClosed,
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
            dateCreated,
            dateClosed,
            dateDeleted,
            dateUpdated);
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
            AccountId.New,
            userId,
            accountName,
            Money.Zero,
            description,
            displayColor,
            overDraftAmount,
            DateTime.UtcNow,
            null,
            null,
            null);
    }

    public void AdjustOverDraftAmount(Money newValue, DateTime dateChanged, string reason = "")
    {
        var previousValue = OverDraftAmount;
        OverDraftAmount = newValue;
        DateUpdated = DateTime.UtcNow;

        AddChangeHistory(
            nameof(OverDraftAmount),
            previousValue,
            newValue,
            dateChanged, reason);

        //AddDomainEvent(new AccountOverDraftAmountAdjustedEvent(Id, userId, newValue));
    }
}

public sealed class SavingsAccount : Account, IHasInterestRate
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

    public void AdjustInterestRate(PercentageRate newValue, DateTime dateChanged, string reason = "")
    {
        var previousValue = InterestRate;
        InterestRate = newValue;
        DateUpdated = DateTime.UtcNow;

        AddChangeHistory(
            nameof(InterestRate),
            previousValue,
            newValue,
            dateChanged,
            reason);

        //AddDomainEvent(new AccountInterestRateAdjustedEvent(Id, userId, newValue));
    }
}

public abstract class CreditAccount : Account, IHasCreditLimit, IHasInterestRate
{
    protected CreditAccount(AccountId id,
        UserId userId,
        AccountTypes.Keys accountType,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated) : base(
        id,
        userId,
        accountType,
        accountName,
        balance,
        description,
        displayColor,
        dateCreated,
        dateClosed,
        dateDeleted,
        dateUpdated)
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
    /// With Credit Accounts, Deposits decrease the Balance (owed).
    /// </summary>
    /// <param name="dateOfDeposit"></param>
    /// <param name="amount"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public override void Deposit(DateTime dateOfDeposit, Money amount)
    {
        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount cannot be negative");

        AddMonthlyTotalCredit(dateOfDeposit, amount);

        Balance -= amount;
    }

    /// <summary>
    /// With Credit Accounts, Withdraws increase the Balance (owed).
    /// </summary>
    /// <param name="dateOfWithdraw"></param>
    /// <param name="amount"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public override void Withdraw(DateTime dateOfWithdraw, Money amount)
    {
        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Withdraw amount cannot be negative");

        AddMonthlyTotalDebit(dateOfWithdraw, amount);

        Balance += amount;
    }

    public void AdjustCreditLimit(Money newValue, DateTime dateChanged, string reason = "")
    {
        var previousValue = CreditLimit;
        CreditLimit = newValue;

        DateUpdated = DateTime.UtcNow;

        AddChangeHistory(
            nameof(CreditLimit),
            previousValue,
            newValue,
            dateChanged,
            reason);

        //AddDomainEvent(new AccountInterestRateAdjustedEvent(Id, userId, newValue));
    }

    public void AdjustInterestRate(PercentageRate newValue, DateTime dateChanged, string reason = "")
    {
        var previousValue = InterestRate;
        InterestRate = newValue;
        DateUpdated = DateTime.UtcNow;

        AddChangeHistory(
            nameof(InterestRate),
            previousValue,
            newValue,
            dateChanged,
            reason);

        //AddDomainEvent(new AccountInterestRateAdjustedEvent(Id, userId, newValue));
    }
}

public sealed class CreditCardAccount : CreditAccount
{
    private CreditCardAccount(
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
            AccountTypes.Keys.CreditCard,
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
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated)
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
            dateCreated,
            dateClosed,
            dateDeleted,
            dateUpdated);
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

public sealed class LineOfCreditAccount : CreditAccount
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