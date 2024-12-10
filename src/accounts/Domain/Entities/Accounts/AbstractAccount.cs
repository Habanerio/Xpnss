using Habanerio.Xpnss.Shared.Entities;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public abstract class AbstractAccountBase :
    AggregateRoot<AccountId>
{


    public UserId UserId { get; }

    /// <summary>
    /// ACCTTYPE
    /// </summary>
    public abstract AccountEnums.AccountKeys AccountType { get; }

    public abstract BankAccountEnums.BankAccountKeys BankAccountType { get; }

    public abstract InvestmentAccountEnums.InvestmentAccountKeys InvestmentAccountType { get; }

    public abstract LoanAccountEnums.LoanAccountKeys LoanAccountType { get; }

    public virtual bool CanBeDeleted => false;

    /// <summary>
    /// Name of the specific Account type
    /// </summary>
    /// <example>Capital One (Credit Card)</example>
    public AccountName Name { get; private set; }

    public Money Balance { get; protected set; }

    public string Description { get; private set; }

    public string DisplayColor { get; private set; }

    public abstract bool IsCredit { get; }

    /// <summary>
    /// Use UpdateDetails to change the Account's IsDefault
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// Use UpdateDetails to change the Account's SortOrder
    /// </summary>
    public int SortOrder { get; private set; }

    public IReadOnlyCollection<TransactionEnums.TransactionKeys> CreditTransactionTypes =>
        IsCredit ? TransactionEnums.CreditTransactionKeys
            : TransactionEnums.DebitTransactionKeys;

    public IReadOnlyCollection<TransactionEnums.TransactionKeys> DebitTransactionTypes =>
        IsCredit ? TransactionEnums.DebitTransactionKeys
            : TransactionEnums.CreditTransactionKeys;

    // New Accounts
    // Adds a Domain Event for the creation of a new Account
    protected AbstractAccountBase(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        bool isDefault = false,
        int? sortOrder = null) :
        base(AccountId.New)
    {
        IsTransient = true;

        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Name = accountName;
        Balance = Money.Zero;
        DisplayColor = displayColor?.Trim() ?? string.Empty;
        Description = description?.Trim() ?? string.Empty;
        DateCreated = DateTime.UtcNow;
        IsDefault = isDefault;
        SortOrder = sortOrder.GetValueOrDefault(-1) < 0 ? 999 : sortOrder!.Value;

        // Add `AccountCreated` Domain Event
    }

    protected AbstractAccountBase(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        bool isDefault,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) : base(id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Name = accountName;
        Balance = new Money(balance);
        DisplayColor = displayColor?.Trim() ?? string.Empty;
        Description = description?.Trim() ?? string.Empty;
        IsDefault = isDefault;
        SortOrder = sortOrder;
        DateCreated = dateCreated;
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
    public abstract void AddTransactionAmount(
        Money amount,
        TransactionEnums.TransactionKeys transactionType);

    /// <summary>
    /// Undoes a previously applied Transaction CreditLimit from the Account's Balance (eg: for when a Transaction is deleted).<br />
    /// When the Transaction is a Credit, the creditLimit will be SUBTRACTED from the Balance.<br />
    /// When the Transaction is a Debit, the creditLimit will be ADDED to the Balance.
    /// </summary>
    /// <param name="amount">The creditLimit of the original Transaction</param>
    /// <param name="transactionType">The original Transaction Type</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public abstract void RemoveTransactionAmount(
        Money amount,
        TransactionEnums.TransactionKeys transactionType);

    /// <summary>
    /// Values will only be updated if they are not null.
    /// </summary>
    /// <param name="accountName"></param>
    /// <param name="description"></param>
    /// <param name="displayColor"></param>
    /// <param name="isDefault"></param>
    /// <param name="sortOrder"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void UpdateDetails(
        string? accountName = null,
        string? description = null,
        string? displayColor = null,
        bool? isDefault = null,
        int? sortOrder = null)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted Account");

        var isDirty = false;

        if (!string.IsNullOrWhiteSpace(accountName) && !Name.Value.Equals(accountName))
        {
            Name = new AccountName(accountName);
            isDirty = true;
        }

        if (!string.IsNullOrWhiteSpace(description) && !Description.Equals(description))
        {
            Description = new AccountName(description);
            isDirty = true;
        }

        if (!string.IsNullOrWhiteSpace(displayColor) && !DisplayColor.Equals(displayColor))
        {
            DisplayColor = displayColor;
            isDirty = true;
        }

        if (isDefault is not null && IsDefault != isDefault)
        {
            IsDefault = isDefault!.Value;
            isDirty = true;
        }

        if (sortOrder is not null && SortOrder != sortOrder && SortOrder > 0)
        {
            SortOrder = sortOrder!.Value;
            isDirty = true;
        }

        if (isDirty)
            DateUpdated = DateTime.UtcNow;
    }
}

public abstract class AbstractAccount : AbstractAccountBase
{
    public DateTime? ClosedDate { get; protected set; }

    /// <summary>
    /// OFX ACCTID
    /// </summary>
    public string ExtAcctId { get; protected set; }

    public bool IsClosed => ClosedDate.HasValue;

    /// <summary>
    /// New Account
    /// </summary>
    protected AbstractAccount(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        string extAcctId,
        bool isDefault,
        int? sortOrder) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            isDefault,
            sortOrder)
    {
        ExtAcctId = extAcctId;

        // Add `AccountCreated` Domain Event
    }

    protected AbstractAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
        bool isDefault,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) :
        base(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,
            isDefault,
            sortOrder,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        ExtAcctId = extAcctId;
        ClosedDate = closedDate;
    }

    public void Close(DateTime dateClosed)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot close a deleted Account");

        if (IsClosed)
            return;

        ClosedDate = dateClosed;//.ToUniversalTime();
        DateUpdated = DateTime.UtcNow;

        //AddDomainEvent(new AccountClosedDomainEvent(Id, userId, dateClosed));
    }

    public void Delete()
    {
        if (!CanBeDeleted)
            throw new InvalidOperationException("Cannot delete this type of Account");

        if (IsDeleted)
            return;

        var now = DateTime.UtcNow;
        DateDeleted = now;
        DateUpdated = now;

        //AddDomainEvent(new AccountDeletedDomainEvent(Id, UserId, now));
    }

    public void Open()
    {
        if (!IsClosed)
            return;

        ClosedDate = null;
        DateUpdated = DateTime.UtcNow;

        //AddDomainEvent(new AccountReOpenedDomainEvent(Id, userId));
    }
}