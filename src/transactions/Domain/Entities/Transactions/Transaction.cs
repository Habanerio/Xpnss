using Habanerio.Xpnss.Shared.Entities;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public enum TransactionStatus
{
    NEW,
    ACTIVE,
    PAID,
    DELETED
}

public class Transaction : AggregateRoot<TransactionId>
{
    private readonly List<TransactionItem> _items;
    private readonly int _maxItemsPerTransaction = 1;
    private readonly List<string> _tags;

    public UserId UserId { get; }

    // TODO: Should I store the AccountName too? Then wouldn't need to query that collection to get the name.
    public AccountId AccountId { get; }

    // TODO: Should I store the CategoryName too? Then wouldn't need to query that collection to get the name.
    public CategoryId CategoryId
    {
        get
        {
            if (_items.Count is 0 or > 1)
                return CategoryId.Empty;

            return _items[0]?.CategoryId ?? CategoryId.Empty;
        }
    }

    // TODO: Should I store the SubCategory too? Then wouldn't need to query that collection to get the name.
    public SubCategoryId SubCategoryId
    {
        get
        {
            if (_items.Count is 0 or > 1)
                return SubCategoryId.Empty;

            return _items[0]?.SubCategoryId ?? SubCategoryId.Empty;
        }
    }

    public string Description { get; }

    public string ExtTransactionNo { get; }

    public bool IsCredit { get; }

    /// <summary>
    /// All transaction under the hood, have at least one 'item'.
    /// </summary>
    public IReadOnlyCollection<TransactionItem> Items => _items.AsReadOnly();

    /// <summary>
    /// The id of the person, company, or Account that is paying or receiving the money.
    /// Optional
    /// </summary>
    // TODO: Should I store the PayerPayeeName too? Then wouldn't need to query that collection to get the name.
    public PayerPayeeId PayerPayeeId { get; }

    //public RefTransactionId RefTransactionId { get; }

    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    public virtual Money TotalAmount => new(Items.Sum(i => i.Amount.Value));

    public DateTime TransactionDate { get; }

    public virtual TransactionStatus Status
    {
        get
        {
            if (IsDeleted)
                return TransactionStatus.DELETED;

            if (IsTransient)
                return TransactionStatus.NEW;

            return TransactionStatus.ACTIVE;
        }
    }

    public TransactionEnums.TransactionKeys TransactionType { get; }

    /// <summary>
    /// For NEW (non-existing) Transactions with a single item.
    /// This sets `IsTransient = true` and adds a `TransactionCreatedDomainEvent` to the domain events.
    /// </summary>
    protected Transaction(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        bool isCredit,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        this(
            TransactionId.New,
            userId,
            accountId,
            description,
            extTransactionNo,
            isCredit,
            item,
            payerPayeeId,
            tags,
            transactionDate,
            transactionType,
            dateCreated: DateTime.UtcNow,
            dateUpdated: null,
            dateDeleted: null)
    {
        IsTransient = true;

        _maxItemsPerTransaction = 1;

        // AddDomainEvent(new TransactionCreatedDomainEvent(Id));
    }

    /// <summary>
    /// For NEW (non-existing) Transactions with multiple items (eg: purchases).
    /// This sets `IsTransient = true` and adds a `TransactionCreatedDomainEvent` to the domain events.
    /// </summary>
    protected Transaction(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        bool isCredit,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        this(
            TransactionId.New,
            userId,
            accountId,
            description,
            extTransactionNo,
            isCredit,
            items,
            payerPayeeId,
            tags,
            transactionDate,
            transactionType,
            dateCreated: DateTime.UtcNow,
            dateUpdated: null,
            dateDeleted: null)
    {
        IsTransient = true;

        _maxItemsPerTransaction = 10;

        // AddDomainEvent(new TransactionCreatedDomainEvent(Id));
    }

    /// <summary>
    /// For EXISTING Transactions with a single item.
    /// </summary>
    protected Transaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        bool isCredit,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) : base(id)
    {
        Id = id;
        UserId = userId;
        AccountId = accountId;
        Description = description;
        ExtTransactionNo = extTransactionNo;
        IsCredit = isCredit;
        PayerPayeeId = payerPayeeId;
        TransactionDate = transactionDate.Date;
        TransactionType = transactionType;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;

        _items = [item];

        _tags = tags?.ToList() ?? [];
    }

    /// <summary>
    /// For EXISTING Transactions with multiple items (eg: Purchases).
    /// </summary>
    protected Transaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        bool isCredit,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) : base(id)
    {
        var itemsList = items?.ToList() ?? [];

        if (!itemsList.Any())
            throw new ArgumentException($"Transaction must have at least one Item");

        Id = id;
        UserId = userId;
        AccountId = accountId;
        TransactionType = transactionType;
        IsCredit = isCredit;
        Description = description;
        ExtTransactionNo = extTransactionNo;
        PayerPayeeId = payerPayeeId;
        //RefTransactionId = refTransactionId;
        TransactionDate = transactionDate.Date;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;

        _items = itemsList;
        _tags = tags?.ToList() ?? [];
    }

    internal void AddTransactionItem(Money amount, CategoryId categoryId, SubCategoryId subCategoryId, string description)
    {
        if (Items.Count >= _maxItemsPerTransaction)
            throw new InvalidOperationException($"Cannot add more than {_maxItemsPerTransaction} items to a transaction.");

        if (IsDeleted)
            throw new InvalidOperationException("Cannot add items to a deleted transaction.");

        _items.Add(TransactionItem.New(amount, categoryId, subCategoryId, description));

        // AddDomainEvent(new TransactionUpdatedDomainEvent(Id, Amount, TotalOwing, TotalPaid));

        // AddDomainEvent(new TransactionItemAddedDomainEvent(Id, amount, categoryId, description));
    }

    public void Delete()
    {
        if (!IsDeleted)
        {
            DateDeleted = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
        }

        // AddDomainEvent(new TransactionDeletedDomainEvent(Id));
    }
}