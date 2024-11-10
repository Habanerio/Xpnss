using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Accounts;

public abstract record MonthlyTotals
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

    protected MonthlyTotals(
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount)
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

public sealed record MonthlyAccountTotals : MonthlyTotals
{
    public AccountId AccountId { get; init; }

    public MonthlyAccountTotals(
        AccountId accountId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) :
        base(userId, year, month, creditTotalAmount, creditCount, debitTotalAmount, debitCount)
    {
        AccountId = accountId;
    }
}

public sealed record MonthlyCategoryTotals : MonthlyTotals
{
    public CategoryId CategoryId { get; init; }

    public MonthlyCategoryTotals(
        CategoryId categoryId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) :
        base(userId, year, month, creditTotalAmount, creditCount, debitTotalAmount, debitCount)
    {
        CategoryId = categoryId;
    }
}

public sealed record MonthlyMerchantTotals : MonthlyTotals
{
    public MerchantId MerchantId { get; init; }

    public MonthlyMerchantTotals(
        MerchantId merchantId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) :
        base(userId, year, month, creditTotalAmount, creditCount, debitTotalAmount, debitCount)
    {
        MerchantId = merchantId;
    }
}