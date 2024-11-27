using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;

namespace Habanerio.Xpnss.PayerPayees.Application.Queries.GetPayerPayeesByIds;

public record GetPayerPayeesByIdsQuery(
    string UserId,
    string[] PayerPayeesIds) : IPayerPayeesQuery<Result<IEnumerable<PayerPayeeDto>>>
{ }