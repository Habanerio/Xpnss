using Habanerio.Xpnss.Totals.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Totals.Application;

public class TotalsService(
    IMediator mediator,
    ILogger<TotalsService> logger) :
    IMonthlyTotalsService
{
    private readonly ILogger<TotalsService> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    public Task<TResult> CommandAsync<TResult>(IMonthlyTotalsCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task CommandAsync(IMonthlyTotalsCommand command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TResult>(IMonthlyTotalsQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(query, cancellationToken);
    }

    public ILogger<IMonthlyTotalsService> Logger => _logger;
}