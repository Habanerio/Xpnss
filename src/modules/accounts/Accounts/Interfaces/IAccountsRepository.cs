using System.Collections.ObjectModel;
using FluentResults;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Accounts.Interfaces;

public interface IAccountsRepository
{
    void Add(AccountDocument account);

    ObjectId Add(AccountDto accountDto);

    Task<Result<AccountDocument>> GetByIdAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default);

    Task<Result<ReadOnlyCollection<ChangeHistory>>> GetChangeHistoryAsync(string userId,
        string accountId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<AccountDocument>>> ListByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<AccountDocument>> UpdateDetailsAsync(
        string userId,
        string accountId,
        string name,
        string description,
        string displayColor,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Save needs to be called after all operations are done
    /// </summary>
    /// <param name="account"></param>
    void Update(AccountDocument account);

    Task<Result> SaveAsync(CancellationToken cancellationToken = default);
}