using Habanerio.Xpnss.Merchants.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Merchants.Application;

public class MerchantsService(IMediator mediator, ILogger<MerchantsService> logger) : IMerchantsService
{
    private readonly ILogger<MerchantsService> _logger = logger ??
                                                            throw new ArgumentNullException(nameof(logger));

    private readonly IMediator _mediator = mediator ??
                                           throw new AbandonedMutexException(nameof(mediator));

    public Task<TResult> CommandAsync<TResult>(IMerchantsCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task CommandAsync(IMerchantsCommand command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TResult>(IMerchantsQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(query, cancellationToken);
    }

    public ILogger<IMerchantsService> Logger { get; }
}