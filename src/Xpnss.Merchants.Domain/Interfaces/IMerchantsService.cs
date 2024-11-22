using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Merchants.Domain.Interfaces;

public interface IMerchantsService
{
    Task<TResult> CommandAsync<TResult>(IMerchantsCommand<TResult> command, CancellationToken cancellationToken = default);

    Task CommandAsync(IMerchantsCommand command, CancellationToken cancellationToken = default);

    Task<TResult> QueryAsync<TResult>(IMerchantsQuery<TResult> query, CancellationToken cancellationToken = default);

    public ILogger<IMerchantsService> Logger { get; }
}