using MediatR;

namespace Habanerio.Xpnss.Transactions.Domain.Interfaces;

public interface ITransactionsQuery<out TResult> : IRequest<TResult>
{
    public string TimeZone { get; set; }
}