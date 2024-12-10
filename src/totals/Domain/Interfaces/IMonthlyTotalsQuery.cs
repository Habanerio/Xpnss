using MediatR;

namespace Habanerio.Xpnss.Totals.Domain.Interfaces;

public interface IMonthlyTotalsQuery<out TResult> : IRequest<TResult> { }