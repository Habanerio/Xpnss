using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.MonthlyTotals.Domain.Entities;

public class MonthlyTotal : AggregateRoot<EntityObjectId>
{
    public UserId UserId { get; init; }

    /// <summary>
    /// EntityId is the Id of the entity that this MonthlyTotal is for.
    /// This is nullable, because transactions may not be assigned an Account, a Category, or a PayerPayee
    /// (think when possibly importing transactions)
    /// </summary>
    public EntityObjectId? EntityId { get; init; }

    /// <summary>
    /// EntityType is the type of the entity that this MonthlyTotal is for.
    /// Eg: Account, Category, etc.
    /// </summary>
    public EntityTypes.Keys EntityType { get; init; }

    public int Month { get; set; }

    public int Year { get; set; }

    /// <summary>
    /// Total amount of money added.
    /// </summary>
    public Money CreditTotalAmount { get; set; }

    /// <summary>
    /// Total number of credits added.
    /// </summary>
    public int CreditCount { get; set; }

    /// <summary>
    /// Total amount of money removed.
    /// </summary>
    public Money DebitTotalAmount { get; set; }

    /// <summary>
    /// Total number of debits removed.
    /// </summary>
    public int DebitCount { get; set; }


    private MonthlyTotal(
        UserId userId,
        EntityObjectId entityId,
        EntityTypes.Keys entityType,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) :
        this(
            EntityObjectId.New,
            userId,
            entityId,
            entityType,
            year,
            month,
            creditTotalAmount,
            creditCount,
            debitTotalAmount,
            debitCount,
            DateTime.UtcNow)
    {
        IsTransient = true;

        // Add `MonthlyTotalsCreated` Domain Event
    }

    private MonthlyTotal(
        EntityObjectId id,
        UserId userId,
        EntityObjectId entityId,
        EntityTypes.Keys entityType,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(id)
    {
        if (EntityObjectId.IsEmpty(entityId))
            throw new ArgumentException($"{nameof(entityId)} cannot be null or whitespace.",
                nameof(entityId));

        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), $"{nameof(month)} must be between 1 and 12.");

        UserId = userId;

        EntityId = entityId;
        EntityType = entityType;

        Month = month;
        Year = year;

        CreditTotalAmount = creditTotalAmount;
        CreditCount = creditCount;
        DebitTotalAmount = debitTotalAmount;
        DebitCount = debitCount;

        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;
    }

    public static MonthlyTotal Load(
        EntityObjectId id,
        UserId userId,
        EntityObjectId? entityId,
        EntityTypes.Keys entityType,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new MonthlyTotal(
            id,
            userId,
            entityId,
            entityType,
            year,
            month,
            creditTotalAmount,
            creditCount,
            debitTotalAmount,
            debitCount,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    public static MonthlyTotal New(
        UserId userId,
        EntityObjectId entityId,
        EntityTypes.Keys entityType,
        int year,
        int month,
        bool isCredit,
        Money amount)
    {
        return new MonthlyTotal(
            userId,
            entityId,
            entityType,
            year,
            month,
            isCredit ? amount : Money.Zero,
            isCredit ? 1 : 0,
            !isCredit ? amount : Money.Zero,
            !isCredit ? 1 : 0);
    }
}