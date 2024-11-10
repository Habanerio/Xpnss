using FluentResults;
using Habanerio.Xpnss.Application.Accounts.DTOs;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;

namespace Habanerio.Xpnss.Application.Accounts.Queries.GetAccounts;

public sealed record GetAccountsQuery(string UserId) : IAccountsQuery<Result<IEnumerable<AccountDto>>>;