using Habanerio.Xpnss.Domain.Events;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Transactions;

public enum TransactionStatus
{
    New,
    Active,
    Paid,
    Deleted
}

public class Transaction : AggregateRoot<TransactionId>
{
    private readonly List<TransactionItem> _items;
    private readonly List<TransactionPayment> _payments;

    private const int MaxItemsPer = 25;

    public UserId UserId { get; init; }

    public AccountId AccountId { get; init; }

    public string Description { get; set; }

    public MerchantId MerchantId { get; init; }

    public bool IsDeleted => DateDeleted.HasValue;

    public IReadOnlyCollection<TransactionItem> Items => _items.AsReadOnly();

    public IReadOnlyCollection<TransactionPayment> Payments => _payments.AsReadOnly();

    public Money TotalAmount => new(_items.Sum(i => i.Amount.Value));

    public Money TotalOwing => TotalAmount - TotalPaid;

    public Money TotalPaid => new(_payments.Sum(p => p.Amount.Value));

    public DateTime TransactionDate { get; init; }

    public TransactionStatus Status
    {
        get
        {
            if (IsDeleted)
                return TransactionStatus.Deleted;

            if (TotalOwing.Value <= 0)
                return TransactionStatus.Paid;

            if (IsTransient)
                return TransactionStatus.New;

            return TransactionStatus.Active;
        }
    }

    public TransactionTypes.Keys TransactionType { get; init; }

    public DateTime? DatePaid => TotalOwing.Value <= 0 ? _payments.Max(p => p.PaymentDate).Date : null;

    public DateTime DateCreated { get; init; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }

    /// <summary>
    /// For NEW (non-existing) Transactions.
    /// This adds a `TransactionCreatedDomainEvent` to the domain events.
    /// </summary>
    private Transaction(
        UserId userId,
        AccountId accountId,
        TransactionTypes.Keys transactionType,
        string description,
        MerchantId merchantId,
        List<TransactionItem> transactionItems,
        List<TransactionPayment>? payments,
        DateTime dateOfTransaction,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        this(TransactionId.New,
            userId,
            accountId,
            transactionType,
            description,
            merchantId,
            transactionItems,
            payments,
            dateOfTransaction,
            dateCreated,
            dateUpdated,
            dateDeleted
            )
    {
        IsTransient = true;

        foreach (var transactionItem in transactionItems)
        {
            //AddDomainEvent(new TransactionItemAddedEvent(Id, amount, categoryId, description));
        }

        AddDomainEvent(new TransactionCreatedDomainEvent
        {
            UserId = userId.Value,
            AccountId = accountId.Value,
            TransactionType = transactionType.ToString(),
            Amount = TotalAmount.Value,
            DateOfTransactionUtc = dateOfTransaction
        });
    }

    /// <summary>
    /// For EXISTING Transactions.
    /// </summary>
    private Transaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        TransactionTypes.Keys transactionType,
        string description,
        MerchantId merchantId,
        List<TransactionItem> transactionItems,
        List<TransactionPayment>? payments,
        DateTime dateOfTransaction,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) : base(id)
    {
        Id = id;
        UserId = userId;
        AccountId = accountId;
        TransactionType = transactionType;
        Description = description;
        MerchantId = merchantId;
        TransactionDate = dateOfTransaction;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;

        _items = transactionItems;
        _payments = payments ?? new List<TransactionPayment>();
    }

    public static Transaction Load(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        TransactionTypes.Keys transactionType,
        string description,
        MerchantId merchantId,
        IEnumerable<TransactionItem> transactionItems,
        IEnumerable<TransactionPayment>? payments,
        DateTime dateOfTransaction,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null,
        DateTime? datePaid = null)
    {
        var transactionItemsList = transactionItems.ToList();

        if (transactionItemsList.Count == 0)
            throw new ArgumentException($"{nameof(transactionItems)} cannot be empty.", nameof(transactionItems));

        var paymentsList = payments?.ToList() ?? [];

        return new Transaction(
            id,
            userId,
            accountId,
            transactionType,
            description,
            merchantId,
            transactionItemsList,
            paymentsList,
            dateOfTransaction,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    public static Transaction New(
        UserId userId,
        AccountId accountId,
        TransactionTypes.Keys transactionType,
        string description,
        MerchantId merchantId,
        List<TransactionItem> transactionItems,
        DateTime dateOfTransaction
        )
    {
        if (transactionItems.Count == 0)
            throw new ArgumentException($"{nameof(transactionItems)} cannot be empty.", nameof(transactionItems));

        var transaction = new Transaction(
            userId,
            accountId,
            transactionType,
            description,
            merchantId,
            transactionItems,
            null,
            dateOfTransaction.ToUniversalTime().Date,
            DateTime.UtcNow);

        return transaction;
    }

    public void AddItem(Money amount, CategoryId categoryId, string description)
    {
        if (Items.Count >= MaxItemsPer)
            throw new InvalidOperationException($"Cannot add more than {MaxItemsPer} items to a transaction.");

        if (IsDeleted)
            throw new InvalidOperationException("Cannot add items to a deleted transaction.");

        _items.Add(TransactionItem.New(amount, categoryId, description));

        // AddDomainEvent(new TransactionUpdatedEvent(Id, TotalAmount, TotalOwing, TotalPaid));
        // AddDomainEvent(new TransactionItemAddedEvent(Id, amount, categoryId, description));
    }

    public Money AddPayment(UserId userId, Money amount, DateTime paymentDate)
    {
        if (amount < 0)
            throw new ArgumentException($"{nameof(amount)} cannot be less than 0.", nameof(amount));

        var paymentToApply = amount > TotalOwing ? TotalOwing : amount;
        var remaining = amount > TotalOwing ? amount - TotalOwing : new Money(0);

        ApplyPayment(paymentToApply, paymentDate);

        // AddDomainEvent(new TransactionPaymentAddedEvent(Id, paymentToApply, paymentDate));

        //if(TotalOwing <= 0)
        // AddDomainEvent(new TransactionPaidEvent(Id, userId, TotalAmount, paymentDate));

        return remaining;
    }

    public void Delete()
    {
        if (!IsDeleted)
        {
            DateDeleted = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
        }

        // AddDomainEvent(new TransactionDeletedEvent(Id));
    }

    private void ApplyPayment(Money paymentAmount, DateTime paymentDate)
    {
        _payments.Add(TransactionPayment.New(paymentAmount, paymentDate));

        DateUpdated = DateTime.UtcNow;
    }
}