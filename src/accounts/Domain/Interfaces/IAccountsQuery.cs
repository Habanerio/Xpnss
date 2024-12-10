using MediatR;

namespace Habanerio.Xpnss.Accounts.Domain.Interfaces;

public interface IAccountsQuery<out TResult> : IRequest<TResult> { }