using FluentResults;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Application.DTOs;

namespace Habanerio.Xpnss.Accounts.Application.Queries.GetAccount;

public record GetAccountQuery(string UserId, string AccountId) : IAccountsQuery<Result<AccountDto>>
{ }