using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Merchants.Domain;

namespace Habanerio.Xpnss.Merchants.Application.Mappers;

internal static partial class Mapper
{
    public static MerchantDto? Map(Merchant? merchant)
    {
        if (merchant is null)
            return null;

        return new MerchantDto(merchant.Id, merchant.UserId, merchant.Name, merchant.Location);
    }

    public static IEnumerable<MerchantDto> Map(IEnumerable<Merchant> merchants)
    {
        return merchants.Select(Map).Where(x => x is not null).Cast<MerchantDto>();
    }
}