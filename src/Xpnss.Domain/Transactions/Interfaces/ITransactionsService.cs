using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Domain.Transactions.Interfaces;

public interface ITransactionsService
{
    Task<TResult> CommandAsync<TResult>(ITransactionsCommand<TResult> command, CancellationToken cancellationToken = default);

    Task CommandAsync(ITransactionsCommand command, CancellationToken cancellationToken = default);

    Task<TResult> QueryAsync<TResult>(ITransactionsQuery<TResult> query, CancellationToken cancellationToken = default);

    public ILogger<ITransactionsService> Logger { get; }
}