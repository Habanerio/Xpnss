using FluentResults;
using Habanerio.Xpnss.Application.Merchants.DTOs;
using Habanerio.Xpnss.Domain.Merchants.Interfaces;

namespace Habanerio.Xpnss.Application.Merchants.Queries.GetMerchantsByIds;

public record GetMerchantsByIdsQuery(string UserId, string[] MerchantIds) : IMerchantsQuery<Result<IEnumerable<MerchantDto>>>
{ }