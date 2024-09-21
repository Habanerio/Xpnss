namespace Habanerio.Core.DBs.EFCore.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}