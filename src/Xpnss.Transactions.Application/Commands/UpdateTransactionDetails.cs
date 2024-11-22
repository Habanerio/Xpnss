using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public class UpdateTransactionDetails
{
    public record Command(
        string UserId,
        string TransactionId,
        string AccountId,
        string Description,
        MerchantDto? Merchant,
        DateTime Date) : IRequest<Result>;
}