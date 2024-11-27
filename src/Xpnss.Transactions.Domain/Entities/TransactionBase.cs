using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities;

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

    public bool IsDeleted => DateDeleted.HasValue;

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

    public DateTime DateCreated { get; init; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }

    /// <summary>
    /// For NEW (non-existing) Transactions.
    /// This sets `IsTransient = true` and adds a `TransactionCreatedDomainEvent` to the domain events.
    /// </summary>
    protected TransactionBase(
        UserId userId,
        AccountId accountId,
        TransactionTypes.Keys transactionType,
        Money amount,
        string description,
        DateTime transactionDate,
        IEnumerable<string>? tags) : base(TransactionId.New)
    {
        IsTransient = true;

        UserId = userId;
        AccountId = accountId;
        TotalAmount = amount;
        Description = description;
        TransactionDate = transactionDate.Date;
        TransactionType = transactionType;
        DateCreated = DateTime.UtcNow;

        _tags = tags?.ToList() ?? [];

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