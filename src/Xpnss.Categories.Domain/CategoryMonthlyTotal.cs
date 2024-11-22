using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Categories.Domain;

public sealed class CategoryMonthlyTotal : MonthlyTotal
{
    public CategoryId CategoryId { get; init; }

    private CategoryMonthlyTotal(
        EntityObjectId id,
        CategoryId categoryId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount) :
        base(id, userId, year, month, creditTotalAmount, creditCount, debitTotalAmount, debitCount)
    {
        if (string.IsNullOrWhiteSpace(categoryId.Value) || categoryId.Value.Equals(ObjectId.Empty.ToString()))
            throw new ArgumentException($"{nameof(categoryId)} cannot be null or whitespace.", nameof(categoryId));

        CategoryId = categoryId;
    }

    public static CategoryMonthlyTotal Load(
        EntityObjectId id,
        CategoryId categoryId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount)
    {
        return new CategoryMonthlyTotal(
            id,
            categoryId,
            userId,
            year,
            month,
            creditTotalAmount,
            creditCount,
            debitTotalAmount,
            debitCount);
    }

    public static CategoryMonthlyTotal New(
        CategoryId categoryId,
        UserId userId,
        int year,
        int month,
        Money creditTotalAmount,
        int creditCount,
        Money debitTotalAmount,
        int debitCount)
    {
        return new CategoryMonthlyTotal(
            EntityObjectId.NewId(),
            categoryId,
            userId,
            year,
            month,
            creditTotalAmount,
            creditCount,
            debitTotalAmount,
            debitCount);
    }
}