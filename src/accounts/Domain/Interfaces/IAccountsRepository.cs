using FluentResults;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

namespace Habanerio.Xpnss.Accounts.Domain.Interfaces;

public interface IAccountsRepository
{

    Task<Result<AbstractAccountBase>> AddAsync(
        AbstractAccountBase account,
        CancellationToken cancellationToken = default);

    Task<Result<AbstractAccountBase?>> GetAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default);

    Task<Result<TType?>> GetAsync<TType>(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default) where TType : AbstractAccountBase;

    Task<Result<IEnumerable<AbstractAccountBase>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Would prefer to have specific Update methods, instead of a generic one.
    /// </summary>
    /// <param name="updatedAccount">The accountBase to be updated.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> UpdateAsync(AbstractAccountBase updatedAccount, CancellationToken cancellationToken = default);
}