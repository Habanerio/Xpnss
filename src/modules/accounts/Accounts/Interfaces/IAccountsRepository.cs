using FluentResults;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Accounts.Interfaces;

public interface IAccountsRepository
{
    ObjectId Add(AccountDto account);

    Task<Result<AccountDto>> GetByIdAsync(string accountId, string userId, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<AccountDto>>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result> SaveAsync(CancellationToken cancellationToken = default);
}