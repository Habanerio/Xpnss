using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts;

public sealed class AccountsService(IMediator mediator) : IAccountsService
{
    public async Task<TResult> ExecuteAsync<TResult>(IAccountsCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result;
    }

    public async Task ExecuteAsync(IAccountsCommand command, CancellationToken cancellationToken = default)
    {
        await mediator.Send(command, cancellationToken);
    }

    public async Task<TResult> ExecuteAsync<TResult>(IAccountsQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(query, cancellationToken);

        return result;
    }
}