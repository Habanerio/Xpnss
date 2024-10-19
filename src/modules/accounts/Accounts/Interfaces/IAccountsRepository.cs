using System.Collections.ObjectModel;
using FluentResults;
using Habanerio.Xpnss.Modules.Accounts.Data;

namespace Habanerio.Xpnss.Modules.Accounts.Interfaces;

public interface IAccountsRepository
{

    Task<Result<AccountDocument>> AddAsync(AccountDocument account, CancellationToken cancellationToken = default);

    Task<Result<AccountDocument>> GetByIdAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default);

    Task<Result<ReadOnlyCollection<ChangeHistory>>> GetChangeHistoryAsync(string userId,
        string accountId,
        string timeZone,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<AccountDocument>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<AccountDocument>> UpdateDetailsAsync(
        string userId,
        string accountId,
        string name,
        string description,
        string displayColor,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(AccountDocument account, CancellationToken cancellationToken);
}