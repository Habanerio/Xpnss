using Habanerio.Xpnss.Modules.Categories.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Categories;

public class CategoriesService(IMediator mediator) : ICategoriesService
{
    public async Task<TResult> ExecuteAsync<TResult>(ICategoriesCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result;
    }

    public async Task ExecuteAsync(ICategoriesCommand command, CancellationToken cancellationToken = default)
    {
        await mediator.Send(command, cancellationToken);
    }

    public async Task<TResult> ExecuteAsync<TResult>(ICategoriesQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(query, cancellationToken);

        return result;
    }
}