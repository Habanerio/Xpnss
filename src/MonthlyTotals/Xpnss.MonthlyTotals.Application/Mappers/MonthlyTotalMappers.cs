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
        var entitiesArray = entities?.ToArray() ?? [];

        if (!entitiesArray.Any())
            throw new ArgumentException("The entities cannot be empty", nameof(entities));

        return entitiesArray.Select(Map)
            .Where(x => x is not null)
            .Cast<MonthlyTotalDto>();
    }
}