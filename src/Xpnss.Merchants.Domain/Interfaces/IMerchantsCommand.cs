using MediatR;

namespace Habanerio.Xpnss.Merchants.Domain.Interfaces;

public interface IMerchantsCommand<out TResult> : IRequest<TResult> { }

public interface IMerchantsCommand : IRequest { }