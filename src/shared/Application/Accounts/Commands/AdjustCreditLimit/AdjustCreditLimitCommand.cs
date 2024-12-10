using FluentResults;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Application.Accounts.Commands.AdjustCreditLimit;

public sealed record AdjustCreditLimitCommand(
    string UserId,
    string AccountId,
    decimal CreditLimit,
    DateTime DateOfChange,
    string Reason = "") : IAccountsCommand<Result<decimal>>, IRequest;