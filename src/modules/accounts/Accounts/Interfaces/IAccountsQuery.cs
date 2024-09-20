using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.Interfaces;

public interface IAccountsQuery<out TResult> : IRequest<TResult> { }