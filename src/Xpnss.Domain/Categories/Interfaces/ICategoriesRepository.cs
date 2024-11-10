using FluentResults;

namespace Habanerio.Xpnss.Domain.Categories.Interfaces;

public interface ICategoriesRepository
{
    Task<Result<Category>> AddAsync(
        Category category,
        CancellationToken cancellationToken = default);

    Task<Result<Category?>> GetAsync(
        string userId,
        string categoryId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Category>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<Category>> UpdateAsync(
        string userId,
        Category category,
        CancellationToken cancellationToken = default);
}