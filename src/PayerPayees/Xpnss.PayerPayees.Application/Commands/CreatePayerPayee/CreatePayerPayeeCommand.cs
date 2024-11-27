using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;

namespace Habanerio.Xpnss.PayerPayees.Application.Commands.CreatePayerPayee;

public record CreatePayerPayeeCommand(
    string UserId,
    string Name,
    string Description,
    string Location) :
    IPayerPayeesCommand<Result<PayerPayeeDto>>;