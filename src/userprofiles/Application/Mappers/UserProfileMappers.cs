using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.UserProfiles.Domain.Entities;

namespace Habanerio.Xpnss.UserProfiles.Application.Mappers;

public static partial class ApplicationMapper
{
    public static UserProfileDto? Map(UserProfile? entity)
    {
        if (entity is null)
            return default;

        return new UserProfileDto(
            entity.Id,
            entity.ExtUserId,
            entity.FirstName,
            entity.LastName,
            entity.Email,
            entity.DateLastSeen);
    }

    public static IEnumerable<UserProfileDto> Map(IEnumerable<UserProfile> entities)
    {
        var entitiesArray = entities?.ToArray() ?? [];

        if (!entitiesArray.Any())
            throw new ArgumentException("The entities cannot be empty", nameof(entities));

        return entitiesArray.Select(Map)
            .Where(x => x is not null)
            .Cast<UserProfileDto>();
    }
}