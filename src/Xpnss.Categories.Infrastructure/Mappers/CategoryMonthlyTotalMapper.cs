using Habanerio.Xpnss.Categories.Domain;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Domain.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Categories.Infrastructure.Mappers;

public static partial class Mapper
{
    public static CategoryMonthlyTotal? Map(CategoryMonthlyTotalDocument? document)
    {
        if (document is null)
            return null;

        return CategoryMonthlyTotal.Load(
            new EntityObjectId(document.Id),
            new CategoryId(document.CategoryId),
            new UserId(document.UserId),
            document.Year,
            document.Month,
            new Money(document.CreditTotal),
            document.CreditCount,
           new Money(document.DebitTotal),
            document.DebitCount);
    }

    public static IEnumerable<CategoryMonthlyTotal> Map(IEnumerable<CategoryMonthlyTotalDocument> documents)
    {
        return documents.Select(Map)
            .Where(x => x is not null)
            .Cast<CategoryMonthlyTotal>();
    }

    public static CategoryMonthlyTotalDocument? Map(CategoryMonthlyTotal? entity)
    {
        if (entity is null)
            return null;

        return new CategoryMonthlyTotalDocument
        {
            Id = ObjectId.Parse(entity.Id),
            UserId = entity.UserId,
            Year = entity.Year,
            Month = entity.Month,
            CreditTotal = entity.CreditTotalAmount,
            CreditCount = entity.CreditCount,
            DebitTotal = entity.DebitTotalAmount,
            DebitCount = entity.DebitCount
        };
    }

    public static List<CategoryMonthlyTotalDocument> Map(IEnumerable<CategoryMonthlyTotal> entities)
    {
        return entities
            .Select(Map)
            .Where(x => x is not null)
            .Cast<CategoryMonthlyTotalDocument>()
            .ToList();
    }
}