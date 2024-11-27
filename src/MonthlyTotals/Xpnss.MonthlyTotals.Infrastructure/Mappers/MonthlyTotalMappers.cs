using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.MonthlyTotals.Domain.Entities;
using Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Documents;
using MongoDB.Bson;

namespace Habanerio.Xpnss.MonthlyTotals.Infrastructure.Mappers;

internal static partial class InfrastructureMapper
{
    public static MonthlyTotal? Map(MonthlyTotalDocument? document)
    {
        if (document is null)
            return null;

        return MonthlyTotal.Load(
            new EntityObjectId(document.Id),
            new UserId(document.UserId),
            document.EntityId,
            document.EntityType,
            document.Year,
            document.Month,
            new Money(document.CreditTotal),
            document.CreditCount,
            new Money(document.DebitTotal),
            document.DebitCount);
    }

    public static IEnumerable<MonthlyTotal> Map(IEnumerable<MonthlyTotalDocument> documents)
    {
        return documents.Select(Map)
            .Where(x => x is not null)
            .Cast<MonthlyTotal>();
    }

    public static MonthlyTotalDocument? Map(MonthlyTotal? entity)
    {
        if (entity is null)
            return null;

        return new MonthlyTotalDocument
        {
            Id = ObjectId.Parse(entity.Id),
            UserId = entity.UserId,
            EntityId = entity.EntityId,
            EntityType = entity.EntityType,
            Year = entity.Year,
            Month = entity.Month,
            CreditTotal = entity.CreditTotalAmount,
            CreditCount = entity.CreditCount,
            DebitTotal = entity.DebitTotalAmount,
            DebitCount = entity.DebitCount
        };
    }

    public static List<MonthlyTotalDocument> Map(IEnumerable<MonthlyTotal> entities)
    {
        return entities
            .Select(Map)
            .Where(x => x is not null)
            .Cast<MonthlyTotalDocument>()
            .ToList();
    }
}