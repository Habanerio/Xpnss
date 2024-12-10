using MediatR;

namespace Habanerio.Xpnss.Totals.Domain.Interfaces;

public interface IMonthlyTotalsCommand<out TResult> : IRequest<TResult> { }

public interface IMonthlyTotalsCommand : IRequest { }