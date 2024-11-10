using MediatR;

namespace Habanerio.Xpnss.Domain.Merchants.Interfaces;

public interface IMerchantsCommand<out TResult> : IRequest<TResult> { }

public interface IMerchantsCommand : IRequest { }