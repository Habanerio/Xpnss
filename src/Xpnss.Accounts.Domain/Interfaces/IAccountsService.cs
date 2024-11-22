using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Accounts.Domain.Interfaces;

public interface IAccountsService
{
    Task<TResult> CommandAsync<TResult>(IAccountsCommand<TResult> command, CancellationToken cancellationToken = default);

    Task CommandAsync(IAccountsCommand command, CancellationToken cancellationToken = default);

    Task<TResult> QueryAsync<TResult>(IAccountsQuery<TResult> query, CancellationToken cancellationToken = default);

    public ILogger<IAccountsService> Logger { get; }
}