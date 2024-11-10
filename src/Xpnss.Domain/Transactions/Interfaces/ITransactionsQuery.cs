using MediatR;

namespace Habanerio.Xpnss.Domain.Transactions.Interfaces;

public interface ITransactionsQuery<out TResult> : IRequest<TResult>
{
    public string TimeZone { get; set; }
}