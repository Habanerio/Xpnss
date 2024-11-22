using FluentResults;

namespace Habanerio.Xpnss.Categories.Domain.Interfaces;

public interface ICategoryMonthlyTotalsRepository
{
    Task<Result<CategoryMonthlyTotal?>> AddAsync(
        CategoryMonthlyTotal accountMonthlyTotal,
        CancellationToken cancellationToken = default);

    Task<Result<CategoryMonthlyTotal?>> GetAsync(
        string accountId,
        string userId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<CategoryMonthlyTotal>>> ListAsync(
        string accountId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<CategoryMonthlyTotal>>> ListAsync(
        string accountId,
        string userId,
        int year,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<CategoryMonthlyTotal>>> ListAsync(
        string accountId,
        string userId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> UpdateAsync(
        CategoryMonthlyTotal accountMonthlyTotal,
        CancellationToken cancellationToken = default);
}