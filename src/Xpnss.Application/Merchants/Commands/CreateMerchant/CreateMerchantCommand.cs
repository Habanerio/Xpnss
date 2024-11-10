using FluentResults;
using Habanerio.Xpnss.Application.Merchants.DTOs;
using Habanerio.Xpnss.Domain.Merchants.Interfaces;

namespace Habanerio.Xpnss.Application.Merchants.Commands.CreateMerchant;

public record CreateMerchantCommand(string UserId, string Name, string Location) : IMerchantsCommand<Result<MerchantDto>>;