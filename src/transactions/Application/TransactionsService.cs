using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Transactions.Application;

public class TransactionsService(IMediator mediator, ILogger<TransactionsService> logger) : ITransactionsService
{
    private readonly ILogger<TransactionsService> _logger = logger ??
            throw new ArgumentNullException(nameof(logger));

    private readonly IMediator _mediator = mediator ??
        throw new AbandonedMutexException(nameof(mediator));

    public Task<TResult> CommandAsync<TResult>(ITransactionsCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task CommandAsync(ITransactionsCommand command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TResult>(ITransactionsQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(query, cancellationToken);
    }

    public ILogger<ITransactionsService> Logger { get; }
}