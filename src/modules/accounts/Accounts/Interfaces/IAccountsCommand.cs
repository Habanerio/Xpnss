using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.Interfaces;

public interface IAccountsCommand<out TResult> : IRequest<TResult> { }

public interface IAccountsCommand : IRequest { }