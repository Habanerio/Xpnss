using FluentResults;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

namespace Habanerio.Xpnss.Accounts.Domain.Interfaces;

public interface IAccountsRepository
{

    Task<Result<BaseAccount>> AddAsync(BaseAccount account, CancellationToken cancellationToken = default);

    Task<Result<BaseAccount?>> GetAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default);

    Task<Result<TType?>> GetAsync<TType>(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default) where TType : BaseAccount;

    Task<Result<IEnumerable<BaseAccount>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Would prefer to have specific Update methods, instead of a generic one.
    /// </summary>
    /// <param name="updatedAccount">The account to be updated.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> UpdateAsync(BaseAccount updatedAccount, CancellationToken cancellationToken = default);
}