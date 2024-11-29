using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.MonthlyTotals.Domain.Entities;

namespace Habanerio.Xpnss.MonthlyTotals.Application.Mappers;

internal static partial class ApplicationMapper
{
    public static MonthlyTotalDto? Map(MonthlyTotal? entity)
    {
        if (entity is null)
            return default;

        return new MonthlyTotalDto()
        {
            EntityId = entity.EntityId ?? string.Empty,
            EntityType = entity.EntityType.ToString(),
            Month = entity.Month,
            Year = entity.Year,
            CreditCount = entity.CreditCount,
            CreditTotalAmount = entity.CreditTotalAmount,
            DebitCount = entity.DebitCount,
            DebitTotalAmount = entity.DebitTotalAmount
        };
    }

    public static IEnumerable<MonthlyTotalDto> Map(IEnumerable<MonthlyTotal> entities)
    {
        if (!entities.TryGetNonEnumeratedCount(out var count) || count == 0)
            return Enumerable.Empty<MonthlyTotalDto>();

        return entities.Select(Map)
            .Where(x => x is not null)
            .Cast<MonthlyTotalDto>();
    }
}