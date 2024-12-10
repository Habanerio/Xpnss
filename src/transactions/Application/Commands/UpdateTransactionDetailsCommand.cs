using FluentResults;
using Habanerio.Xpnss.Shared.DTOs;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public class UpdateTransactionDetails
{
    public sealed record Command(
        string UserId,
        string TransactionId,
        string AccountId,
        string Description,
        PayerPayeeDto? Merchant,
        DateTime Date) : IRequest<Result>;
}