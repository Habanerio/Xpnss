using MediatR;

namespace Habanerio.Xpnss.Domain.Accounts.Interfaces;

public interface IAccountsCommand<out TResult> : IRequest<TResult> { }

public interface IAccountsCommand : IRequest { }