using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.MonthlyTotals.Domain.Entities;

public class MonthlyTotal : AggregateRoot<EntityObjectId>
{
    public UserId UserId { get; init; }

    /// <summary>
    /// EntityId is the Id of the entity that this MonthlyTotal is for.
    /// </summary>
    public string EntityId { get; init; }

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
        string entityId,
        EntityTypes.Keys entityType,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) :
        this(EntityObjectId.Empty, userId, entityId, entityType, year, month, creditTotalAmount, creditCount, debitTotalAmount, debitCount)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException($"{nameof(entityId)} cannot be null or whitespace.", nameof(entityId));

        // AddDomainEvent(new MonthlyTotalCreatedEvent(this)) ?;
    }

    private MonthlyTotal(
        EntityObjectId id,
        UserId userId,
        string entityId,
        EntityTypes.Keys entityType,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) :
        base(id)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException($"{nameof(entityId)} cannot be null or whitespace.", nameof(entityId));

        UserId = userId;

        EntityId = entityId;
        EntityType = entityType;

        Month = month;
        Year = year;

        CreditTotalAmount = creditTotalAmount;
        CreditCount = creditCount;
        DebitTotalAmount = debitTotalAmount;
        DebitCount = debitCount;
    }

    public static MonthlyTotal Load(
        EntityObjectId id,
        UserId userId,
        string entityId,
        EntityTypes.Keys entityType,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount)
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
            debitCount);
    }

    public static MonthlyTotal New(
        UserId userId,
        string entityId,
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

    //public static MonthlyTotal New(
    //    EntityObjectId entityId,
    //    UserId userId,
    //    int year,
    //    int month,
    //    Money creditTotalAmount,
    //    int creditCount,
    //    Money debitTotalAmount,
    //    int debitCount)
    //{
    //    return new MonthlyTotal(
    //        EntityObjectId.New,
    //        entityId,
    //        userId,
    //        year,
    //        month,
    //        creditTotalAmount,
    //        creditCount,
    //        debitTotalAmount,
    //        debitCount);
    //}
}