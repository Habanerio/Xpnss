using FluentResults;
using Habanerio.Xpnss.Accounts.Domain.Entities;

namespace Habanerio.Xpnss.Accounts.Domain.Interfaces;

public interface IAccountMonthlyTotalsRepository
{
    Task<Result<AccountMonthlyTotal?>> AddAsync(
        AccountMonthlyTotal accountMonthlyTotal,
        CancellationToken cancellationToken = default);

    Task<Result<AccountMonthlyTotal?>> GetAsync(
        string userId,
        string accountId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AccountMonthlyTotal>> ListAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AccountMonthlyTotal>> ListAsync(
        string userId,
        string accountId,
        int year,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AccountMonthlyTotal>> ListAsync(
        string userId,
        string accountId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<Result<AccountMonthlyTotal>> UpdateAsync(
        AccountMonthlyTotal accountMonthlyTotal,
        CancellationToken cancellationToken = default);
}