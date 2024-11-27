using MediatR;

namespace Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;

public interface IMonthlyTotalsCommand<out TResult> : IRequest<TResult> { }

public interface IMonthlyTotalsCommand : IRequest { }