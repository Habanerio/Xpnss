using FluentResults;
using Habanerio.Xpnss.Modules.Categories.Data;

namespace Habanerio.Xpnss.Modules.Categories.Interfaces;

public interface ICategoriesRepository
{
    Task<Result<CategoryDocument>> AddAsync(
        CategoryDocument category,
        CancellationToken cancellationToken = default);

    Task<Result<CategoryDocument>> GetByIdAsync(
        string userId,
        string categoryId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<CategoryDocument>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<CategoryDocument>> UpdateAsync(
        string userId,
        CategoryDocument category,
        CancellationToken cancellationToken = default);
}