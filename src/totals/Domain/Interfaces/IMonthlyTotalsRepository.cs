using FluentResults;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Totals.Domain.Entities;

namespace Habanerio.Xpnss.Totals.Domain.Interfaces;

public interface IMonthlyTotalsRepository
{
    Task<Result<MonthlyTotal?>> AddAsync(
        MonthlyTotal monthlyTotal,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single Monthly Total for a given user, entity, year, and month.
    /// </summary>
    /// <returns></returns>
    Task<Result<MonthlyTotal?>> GetAsync(
        string userId,
        string entityId,
        string subEntityId,
        EntityEnums.Keys entityType,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a List of Monthly Totals for a given user, entity, and year.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="entityId"></param>
    /// <param name="entityType"></param>
    /// <param name="year"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityEnums.Keys entityType,
        int year,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a List of Monthly Totals for a given user, entity, and year/month range.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="entityId"></param>
    /// <param name="entityType"></param>
    /// <param name="startMonth"></param>
    /// <param name="endMonth"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<IEnumerable<MonthlyTotal>>> RangeAsync(
        string userId,
        string entityId,
        EntityEnums.Keys entityType,
        (int Year, int Month) startMonth,
        (int Year, int Month) endMonth,
        CancellationToken cancellationToken = default);

    //Task<Result<MonthlyTotal>> UpdateAsync(
    //    MonthlyTotal monthlyTotal,
    //    CancellationToken cancellationToken = default);
}