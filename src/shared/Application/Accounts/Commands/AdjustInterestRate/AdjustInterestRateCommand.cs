using FluentResults;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Application.Accounts.Commands.AdjustInterestRate;

public sealed record AdjustInterestRateCommand(
    string UserId,
    string AccountId,
    decimal InterestRate,
    DateTime DateOfChange,
    string Reason = "") : IAccountsCommand<Result<decimal>>, IRequest;