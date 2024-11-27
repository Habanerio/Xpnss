using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.PayerPayees.Application;

public class PayerPayeesService(IMediator mediator, ILogger<PayerPayeesService> logger) : IPayerPayeesService
{
    private readonly ILogger<PayerPayeesService> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    private readonly IMediator _mediator = mediator ??
        throw new AbandonedMutexException(nameof(mediator));

    public async Task<TResult> CommandAsync<TResult>(IPayerPayeesCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(command, cancellationToken);
        //_logger.LogError(e, "Error executing command {Command}", command.GetType().Name);
    }

    public Task CommandAsync(IPayerPayeesCommand command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TResult>(IPayerPayeesQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(query, cancellationToken);
    }
}