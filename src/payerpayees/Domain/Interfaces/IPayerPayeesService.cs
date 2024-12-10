namespace Habanerio.Xpnss.PayerPayees.Domain.Interfaces;

public interface IPayerPayeesService
{
    Task<TResult> CommandAsync<TResult>(
        IPayerPayeesCommand<TResult> command,
        CancellationToken cancellationToken = default);

    Task CommandAsync(
        IPayerPayeesCommand command,
        CancellationToken cancellationToken = default);

    Task<TResult> QueryAsync<TResult>(
        IPayerPayeesQuery<TResult> query,
        CancellationToken cancellationToken = default);
}