using FluentResults;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Application.Accounts.Commands.AddTransactionAmount;

public record AddTransactionAmountCommand(
    string UserId,
    string AccountId,
    decimal Amount,
    bool IsCredit,
    DateTime DateOfTransaction,
    string Reason = "") : IAccountsCommand<Result>, IRequest;