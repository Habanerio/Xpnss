using MediatR;

namespace Habanerio.Xpnss.Accounts.Domain.Interfaces;

public interface IAccountsCommand<out TResult> : IRequest<TResult> { }

public interface IAccountsCommand : IRequest { }