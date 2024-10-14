namespace Habanerio.Xpnss.Modules.Transactions.Interfaces;

public interface ITransactionsService
{
    Task<TResult> ExecuteAsync<TResult>(ITransactionsCommand<TResult> command, CancellationToken cancellationToken = default);

    Task ExecuteAsync(ITransactionsCommand command, CancellationToken cancellationToken = default);

    Task<TResult> ExecuteAsync<TResult>(ITransactionsQuery<TResult> query, CancellationToken cancellationToken = default);
}