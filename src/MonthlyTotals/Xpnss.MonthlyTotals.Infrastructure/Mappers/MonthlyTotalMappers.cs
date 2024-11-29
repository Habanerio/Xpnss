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
            return default;

        return MonthlyTotal.Load(
            new EntityObjectId(document.Id),
            new UserId(document.UserId),
            document.EntityId != null ? new EntityObjectId(document.EntityId.Value) : null,
            document.EntityType,
            document.Year,
            document.Month,
            new Money(document.CreditTotal),
            document.CreditCount,
            new Money(document.DebitTotal),
            document.DebitCount,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
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
            return default;

        return new MonthlyTotalDocument
        {
            Id = entity.Id,
            UserId = entity.UserId,
            EntityId = entity.EntityId?.Value is not null ?
                ObjectId.Parse(entity.EntityId.Value) :
                null,
            EntityType = entity.EntityType,
            Year = entity.Year,
            Month = entity.Month,
            CreditTotal = entity.CreditTotalAmount,
            CreditCount = entity.CreditCount,
            DebitTotal = entity.DebitTotalAmount,
            DebitCount = entity.DebitCount,
            DateCreated = entity.DateCreated,
            DateUpdated = entity.DateUpdated,
            DateDeleted = entity.DateDeleted
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