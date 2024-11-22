using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Merchants.Domain.Interfaces;

namespace Habanerio.Xpnss.Merchants.Application.Queries.GetMerchant;

public record GetMerchantQuery(string UserId, string MerchantId) : IMerchantsQuery<Result<MerchantDto>>
{ }