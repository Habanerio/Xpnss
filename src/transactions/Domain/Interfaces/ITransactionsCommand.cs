using MediatR;

namespace Habanerio.Xpnss.Transactions.Domain.Interfaces;

public interface ITransactionsCommand<out TResult> : IRequest<TResult> { }

public interface ITransactionsCommand : IRequest { }