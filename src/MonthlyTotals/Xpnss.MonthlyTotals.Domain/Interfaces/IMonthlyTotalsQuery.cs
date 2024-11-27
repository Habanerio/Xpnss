using MediatR;

namespace Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;

public interface IMonthlyTotalsQuery<out TResult> : IRequest<TResult> { }