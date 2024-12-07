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

    public string ExtTransactionId { get; }

    public bool IsCredit { get; private set; }

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

    public TransactionEnums.TransactionKeys TransactionType { get; }

    /// <summary>
    /// For NEW (non-existing) Transactions.
    /// This sets `IsTransient = true` and adds a `TransactionCreatedDomainEvent` to the domain events.
    /// </summary>
    protected TransactionBase(
        UserId userId,
        AccountId accountId,
        TransactionEnums.TransactionKeys transactionType,
        bool isCredit,
        PayerPayeeId payerPayeeId,
        Money amount,
        string description,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "") :
        this(
            TransactionId.New,
            userId,
            accountId,
            transactionType,
            isCredit,
            payerPayeeId,
            amount,
            description,
            transactionDate,
            tags,
            extTransactionId,
            DateTime.UtcNow,
            null,
            null)
    {
        IsTransient = true;

        IsCredit = isCredit;

        // AddDomainEvent(new TransactionCreatedDomainEvent(Id));
    }

    /// <summary>
    /// For EXISTING Transactions.
    /// </summary>
    protected TransactionBase(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        TransactionEnums.TransactionKeys transactionType,
        bool isCredit,
        PayerPayeeId payerPayeeId,
        Money amount,
        string description,
        DateTime transactionDate,
        IEnumerable<string>? tags,
        string extTransactionId,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) : base(id)
    {
        Id = id;
        UserId = userId;
        AccountId = accountId;
        TransactionType = transactionType;
        IsCredit = isCredit;
        Description = description;
        ExtTransactionId = extTransactionId;
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