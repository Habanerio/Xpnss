using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Domain.Categories.Interfaces;

public interface ICategoriesService
{
    Task<TResult> CommandAsync<TResult>(ICategoriesCommand<TResult> command, CancellationToken cancellationToken = default);

    Task CommandAsync(ICategoriesCommand command, CancellationToken cancellationToken = default);

    Task<TResult> QueryAsync<TResult>(ICategoriesQuery<TResult> query, CancellationToken cancellationToken = default);

    public ILogger<ICategoriesService> Logger { get; }
}