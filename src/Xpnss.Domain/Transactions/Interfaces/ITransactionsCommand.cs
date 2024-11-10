using MediatR;

namespace Habanerio.Xpnss.Domain.Transactions.Interfaces;

public interface ITransactionsCommand<out TResult> : IRequest<TResult> { }

public interface ITransactionsCommand : IRequest { }