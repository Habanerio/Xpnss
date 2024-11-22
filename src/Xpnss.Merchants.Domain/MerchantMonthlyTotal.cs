using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Merchants.Domain;

public sealed class MerchantMonthlyTotal : MonthlyTotal
{
    public MerchantId MerchantId { get; init; }

    private MerchantMonthlyTotal(
        EntityObjectId id,
        MerchantId merchantId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) :
        base(id, userId, year, month, creditTotalAmount, creditCount, debitTotalAmount, debitCount)
    {
        if (string.IsNullOrWhiteSpace(merchantId.Value) || merchantId.Value.Equals(ObjectId.Empty.ToString()))
            throw new ArgumentException($"{nameof(merchantId)} cannot be null or whitespace.", nameof(merchantId));

        MerchantId = merchantId;
    }

    public static MerchantMonthlyTotal Load(
        EntityObjectId id,
        MerchantId merchantId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount)
    {
        return new MerchantMonthlyTotal(
            id,
            merchantId,
            userId,
            year,
            month,
            creditTotalAmount,
            creditCount,
            debitTotalAmount,
            debitCount);
    }

    public static MerchantMonthlyTotal New(
        MerchantId merchantId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount)
    {
        return new MerchantMonthlyTotal(
            EntityObjectId.NewId(),
            merchantId,
            userId,
            year,
            month,
            creditTotalAmount,
            creditCount,
            debitTotalAmount,
            debitCount);
    }
}