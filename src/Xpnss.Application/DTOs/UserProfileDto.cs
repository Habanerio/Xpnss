namespace Habanerio.Xpnss.Application.DTOs;

public record UserProfileDto(
    string Id,
    string ExtUserId,
    string FirstName,
    string LastName,
    string Email,
    DateTime DateLastSeen);