using FluentResults;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.MonthlyTotals.Domain.Entities;

namespace Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;

public interface IMonthlyTotalsRepository
{
    Task<Result<MonthlyTotal?>> AddAsync(
        MonthlyTotal monthlyTotal,
        CancellationToken cancellationToken = default);

    Task<Result<MonthlyTotal?>> GetAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        int year,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        (int Year, int Month) startMonth,
        (int Year, int Month) endMonth,
        CancellationToken cancellationToken = default);

    Task<Result<MonthlyTotal>> UpdateAsync(
        MonthlyTotal monthlyTotal,
        CancellationToken cancellationToken = default);
}