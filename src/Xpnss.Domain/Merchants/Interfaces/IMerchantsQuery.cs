using MediatR;

namespace Habanerio.Xpnss.Domain.Merchants.Interfaces;

public interface IMerchantsQuery<out TResult> : IRequest<TResult>
{ }