namespace Habanerio.Xpnss.Application.DTOs;

public sealed record PayerPayeeDto(
    string Id,
    string UserId,
    string Name,
    string Description,
    string Location);