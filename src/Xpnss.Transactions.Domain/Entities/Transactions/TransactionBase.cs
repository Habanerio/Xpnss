using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public enum TransactionStatus
{
    NEW,
    ACTIVE,
    PAID,
    DELETED
}

public class TransactionBase : AggregateRoot<TransactionId>
{
    private readonly List<string> _tags;

    public UserId UserId { get; }

    public AccountId AccountId { get; }

    public string Description { get; }

    /// <summary>
    /// The id of the person, company, or account that is paying or receiving the money.
    /// Optional
    /// </summary>
    public PayerPayeeId PayerPayeeId { get; }

    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    public virtual Money TotalAmount { get; }

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

    public TransactionTypes.Keys TransactionType { get; }

    /// <summary>
    /// For NEW (non-existing) Transactions.
    /// This sets `IsTransient = true` and adds a `TransactionCreatedDomainEvent` to the domain events.
    /// </summary>
    protected TransactionBase(
        UserId userId,
        AccountId accountId,
        TransactionTypes.Keys transactionType,
        PayerPayeeId payerPayeeId,
        Money amount,
        string description,
        DateTime transactionDate,
        IEnumerable<string>? tags) : this(
            TransactionId.Empty,
            userId,
            accountId,
            transactionType,
            payerPayeeId,
            amount,
            description,
            transactionDate,
            tags,
            DateTime.UtcNow)
    {
        IsTransient = true;

        // AddDomainEvent(new TransactionCreatedDomainEvent(Id));
    }

    /// <summary>
    /// For EXISTING Transactions.
    /// </summary>
    protected TransactionBase(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        TransactionTypes.Keys transactionType,
        PayerPayeeId payerPayeeId,
        Money amount,
        string description,
        DateTime transactionDate,
        IEnumerable<string>? tags,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) : base(id)
    {
        Id = id;
        UserId = userId;
        AccountId = accountId;
        TransactionType = transactionType;
        Description = description;
        PayerPayeeId = payerPayeeId;
        TotalAmount = amount;
        TransactionDate = transactionDate.Date;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;

        _tags = tags?.ToList() ?? [];
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