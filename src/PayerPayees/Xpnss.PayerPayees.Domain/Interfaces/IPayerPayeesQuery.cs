using MediatR;

namespace Habanerio.Xpnss.PayerPayees.Domain.Interfaces;

public interface IPayerPayeesQuery<out TResult> : IRequest<TResult>
{ }