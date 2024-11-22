using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Merchants.Domain.Interfaces;

namespace Habanerio.Xpnss.Merchants.Application.Commands.CreateMerchant;

public record CreateMerchantCommand(string UserId, string Name, string Location) : IMerchantsCommand<Result<MerchantDto>>;