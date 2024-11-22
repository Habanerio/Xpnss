using Habanerio.Xpnss.Categories.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Categories.Application;

public class CategoriesService(IMediator mediator, ILogger<CategoriesService> logger) : ICategoriesService
{
    private readonly ILogger<CategoriesService> _logger = logger ??
                                                          throw new ArgumentNullException(nameof(logger));

    private readonly IMediator _mediator = mediator ??
                                           throw new ArgumentNullException(nameof(mediator));


    public Task<TResult> CommandAsync<TResult>(ICategoriesCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task CommandAsync(ICategoriesCommand command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TResult>(ICategoriesQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(query, cancellationToken);
    }

    public ILogger<ICategoriesService> Logger => _logger;
}