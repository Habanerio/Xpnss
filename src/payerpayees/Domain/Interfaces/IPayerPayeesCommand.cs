using MediatR;

namespace Habanerio.Xpnss.PayerPayees.Domain.Interfaces;

public interface IPayerPayeesCommand<out TResult> : IRequest<TResult> { }

public interface IPayerPayeesCommand : IRequest { }