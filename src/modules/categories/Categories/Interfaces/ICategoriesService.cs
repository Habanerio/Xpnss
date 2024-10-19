namespace Habanerio.Xpnss.Modules.Categories.Interfaces;

public interface ICategoriesService
{
    Task<TResult> ExecuteAsync<TResult>(ICategoriesCommand<TResult> command, CancellationToken cancellationToken = default);

    Task ExecuteAsync(ICategoriesCommand command, CancellationToken cancellationToken = default);

    Task<TResult> ExecuteAsync<TResult>(ICategoriesQuery<TResult> query, CancellationToken cancellationToken = default);
}