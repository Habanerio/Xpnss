using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Accounts.Application;

public sealed class AccountsService(IMediator mediator, ILogger<AccountsService> logger) : IAccountsService
{
    private readonly ILogger<AccountsService> _logger = logger ??
                                                        throw new ArgumentNullException(nameof(logger));

    private readonly IMediator _mediator = mediator ??
                                 throw new ArgumentNullException(nameof(mediator));

    public Task<TResult> CommandAsync<TResult>(IAccountsCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task CommandAsync(IAccountsCommand command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TResult>(IAccountsQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(query, cancellationToken);
    }

    public ILogger<IAccountsService> Logger => _logger;
}