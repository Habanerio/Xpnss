using FluentResults;
using Habanerio.Xpnss.Application.Accounts.DTOs;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;

namespace Habanerio.Xpnss.Application.Accounts.Queries.GetAccount;

public record GetAccountQuery(string UserId, string AccountId) : IAccountsQuery<Result<AccountDto>>
{ }