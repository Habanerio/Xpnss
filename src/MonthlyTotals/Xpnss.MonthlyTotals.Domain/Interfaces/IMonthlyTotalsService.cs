using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;

public interface IMonthlyTotalsService
{
    Task<TResult> CommandAsync<TResult>(IMonthlyTotalsCommand<TResult> command, CancellationToken cancellationToken = default);

    Task CommandAsync(IMonthlyTotalsCommand command, CancellationToken cancellationToken = default);

    Task<TResult> QueryAsync<TResult>(IMonthlyTotalsQuery<TResult> query, CancellationToken cancellationToken = default);

    public ILogger<IMonthlyTotalsService> Logger { get; }
}