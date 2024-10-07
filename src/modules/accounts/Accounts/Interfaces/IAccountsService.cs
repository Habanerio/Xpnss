namespace Habanerio.Xpnss.Modules.Accounts.Interfaces;

public interface IAccountsService
{
    Task<TResult> ExecuteAsync<TResult>(IAccountsCommand<TResult> command, CancellationToken cancellationToken = default);

    Task ExecuteAsync(IAccountsCommand command, CancellationToken cancellationToken = default);

    Task<TResult> ExecuteAsync<TResult>(IAccountsQuery<TResult> query, CancellationToken cancellationToken = default);
}