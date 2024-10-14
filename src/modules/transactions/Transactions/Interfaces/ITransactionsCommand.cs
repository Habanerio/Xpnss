using MediatR;

namespace Habanerio.Xpnss.Modules.Transactions.Interfaces;

public interface ITransactionsCommand<out TResult> : IRequest<TResult> { }

public interface ITransactionsCommand : IRequest { }