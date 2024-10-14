using MediatR;

namespace Habanerio.Xpnss.Modules.Transactions.Interfaces;

public interface ITransactionsQuery<out TResult> : IRequest<TResult> { }