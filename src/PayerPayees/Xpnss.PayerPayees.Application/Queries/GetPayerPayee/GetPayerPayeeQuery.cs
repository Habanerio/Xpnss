using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;

namespace Habanerio.Xpnss.PayerPayees.Application.Queries.GetPayerPayee;

public sealed record GetPayerPayeeQuery(string UserId, string PayerPayeeId) :
    IPayerPayeesQuery<Result<PayerPayeeDto>>;