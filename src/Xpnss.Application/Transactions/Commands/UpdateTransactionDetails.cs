using FluentResults;
using MediatR;

namespace Habanerio.Xpnss.Application.Transactions.Commands;

public class UpdateTransactionDetails
{
    public record Command(
        string UserId,
        string TransactionId,
        string Description,
        DateTime Date,
        string AccountId) : IRequest<Result>;
}