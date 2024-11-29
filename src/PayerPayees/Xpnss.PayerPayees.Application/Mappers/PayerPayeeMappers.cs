using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.PayerPayees.Domain.Entities;

namespace Habanerio.Xpnss.PayerPayees.Application.Mappers;

internal static partial class ApplicationMapper
{
    public static PayerPayeeDto? Map(PayerPayee? payerPayee)
    {
        if (payerPayee is null)
            return default;

        return new PayerPayeeDto(
            payerPayee.Id,
            payerPayee.UserId,
            payerPayee.Name,
            payerPayee.Description,
            payerPayee.Location);
    }

    public static IEnumerable<PayerPayeeDto> Map(IEnumerable<PayerPayee> payerPayees)
    {
        return payerPayees.Select(Map)
            .Where(x => x is not null)
            .Cast<PayerPayeeDto>();
    }
}