using FluentResults;
using Habanerio.Xpnss.Application.Merchants.DTOs;
using Habanerio.Xpnss.Domain.Merchants.Interfaces;

namespace Habanerio.Xpnss.Application.Merchants.Queries.GetMerchant;

public record GetMerchantQuery(string UserId, string MerchantId) : IMerchantsQuery<Result<MerchantDto>>
{ }