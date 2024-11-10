using MediatR;

namespace Habanerio.Xpnss.Domain.Accounts.Interfaces;

public interface IAccountsQuery<out TResult> : IRequest<TResult> { }