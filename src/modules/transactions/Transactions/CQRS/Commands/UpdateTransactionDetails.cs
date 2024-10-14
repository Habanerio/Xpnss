using FluentResults;
using MediatR;

namespace Habanerio.Xpnss.Modules.Transactions.CQRS.Commands;

public class UpdateTransactionDetails
{
    public record Command(
        string UserId,
        string TransactionId,
        string Description,
        DateTimeOffset Date,
        string AccountId) : IRequest<Result>;
}