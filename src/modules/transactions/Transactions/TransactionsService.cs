using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Transactions;

public class TransactionsService(IMediator mediator) : ITransactionsService
{
    public async Task<TResult> ExecuteAsync<TResult>(ITransactionsCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result;
    }

    public async Task ExecuteAsync(ITransactionsCommand command, CancellationToken cancellationToken = default)
    {
        await mediator.Send(command, cancellationToken);
    }

    public async Task<TResult> ExecuteAsync<TResult>(ITransactionsQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(query, cancellationToken);

        return result;
    }
}