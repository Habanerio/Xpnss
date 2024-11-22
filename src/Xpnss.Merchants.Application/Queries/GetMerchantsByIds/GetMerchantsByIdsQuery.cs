using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Merchants.Domain.Interfaces;

namespace Habanerio.Xpnss.Merchants.Application.Queries.GetMerchantsByIds;

public record GetMerchantsByIdsQuery(string UserId, string[] MerchantIds) : IMerchantsQuery<Result<IEnumerable<MerchantDto>>>
{ }