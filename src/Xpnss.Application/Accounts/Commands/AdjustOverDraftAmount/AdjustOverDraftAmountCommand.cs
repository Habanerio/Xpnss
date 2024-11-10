using FluentResults;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Application.Accounts.Commands.AdjustOverDraftAmount;

/// <summary>
/// This is for manual changes to the Overdraft Amount, which is logged.
/// 
/// This should have its own Api endpoint.
/// </summary>
public sealed record AdjustOverDraftAmountCommand(
    string UserId,
    string AccountId,
    decimal OverDraftAmount,
    DateTime DateOfChange,
    string Reason = "") : IAccountsCommand<Result<decimal>>, IRequest;