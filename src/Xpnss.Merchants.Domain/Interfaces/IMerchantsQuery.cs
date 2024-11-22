using MediatR;

namespace Habanerio.Xpnss.Merchants.Domain.Interfaces;

public interface IMerchantsQuery<out TResult> : IRequest<TResult>
{ }