using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Accounts.Domain.Entities;

public class AccountMonthlyTotal : MonthlyTotal
{
    public AccountId AccountId { get; init; }

    private AccountMonthlyTotal(
        EntityObjectId id,
        AccountId accountId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) :
        base(id, userId, year, month, creditTotalAmount, creditCount, debitTotalAmount, debitCount)
    {
        if (string.IsNullOrWhiteSpace(accountId.Value) || accountId.Value.Equals(ObjectId.Empty.ToString()))
            throw new ArgumentException($"{nameof(accountId)} cannot be null or whitespace.", nameof(accountId));

        AccountId = accountId;
    }

    public static AccountMonthlyTotal Load(
        EntityObjectId id,
        AccountId accountId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount)
    {
        return new AccountMonthlyTotal(
            id,
            accountId,
            userId,
            year,
            month,
            creditTotalAmount,
            creditCount,
            debitTotalAmount,
            debitCount);
    }

    public static AccountMonthlyTotal New(
        AccountId accountId,
        UserId userId,
        int year,
        int month,
        bool isCredit,
        Money amount)
    {
        return new AccountMonthlyTotal(
            EntityObjectId.NewId(),
            accountId,
            userId,
            year,
            month,
            isCredit ? amount : Money.Zero,
            isCredit ? 1 : 0,
            !isCredit ? amount : Money.Zero,
            !isCredit ? 1 : 0);
    }

    public static AccountMonthlyTotal New(
        AccountId accountId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount)
    {
        return new AccountMonthlyTotal(
            EntityObjectId.NewId(),
            accountId,
            userId,
            year,
            month,
            creditTotalAmount,
            creditCount,
            debitTotalAmount,
            debitCount);
    }
}