using FluentResults;

namespace Habanerio.Xpnss.Domain.Accounts.Interfaces;

public interface IAccountsRepository
{

    Task<Result<Account>> AddAsync(Account account, CancellationToken cancellationToken = default);

    Task<Result<Account?>> GetAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default);

    Task<Result<TType?>> GetAsync<TType>(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default) where TType : Account;

    Task<Result<IEnumerable<Account>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Would prefer to have specific Update methods, instead of a generic one.
    /// </summary>
    /// <param name="account"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> UpdateAsync(Account account, CancellationToken cancellationToken = default);
}