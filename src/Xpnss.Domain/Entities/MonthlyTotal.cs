using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Entities;

public abstract class MonthlyTotal : Entity<EntityObjectId>
{
    public UserId UserId { get; init; }

    public int Year { get; set; }

    public int Month { get; set; }

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

    protected MonthlyTotal(
        EntityObjectId id,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) : base(id)
    {
        UserId = userId;
        Year = year;
        Month = month;
        CreditTotalAmount = creditTotalAmount;
        CreditCount = creditCount;
        DebitTotalAmount = debitTotalAmount;
        DebitCount = debitCount;
    }
}