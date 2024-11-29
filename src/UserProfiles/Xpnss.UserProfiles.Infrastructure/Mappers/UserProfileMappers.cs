using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.UserProfiles.Domain.Entities;
using Habanerio.Xpnss.UserProfiles.Infrastructure.Data.Documents;

namespace Habanerio.Xpnss.UserProfiles.Infrastructure.Mappers;

public static partial class InfrastructureMapper
{
    public static UserProfile? Map(UserProfileDocument? document)
    {
        if (document is null)
            return default;

        return UserProfile.Load(
            new UserId(document.Id),
            document.ExtUserId,
            document.FirstName,
            document.LastName,
            document.Email,
            document.DateLastSeen,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
    }

    public static IEnumerable<UserProfile> Map(IEnumerable<UserProfileDocument> documents)
    {
        return documents.Select(Map)
            .Where(x => x is not null)
            .Cast<UserProfile>();
    }

    public static UserProfileDocument? Map(UserProfile? entity)
    {
        if (entity is null)
            return default;

        return new UserProfileDocument
        {
            Id = entity.Id,
            ExtUserId = entity.ExtUserId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            IsDeleted = entity.IsDeleted,
            DateLastSeen = entity.DateLastSeen,
            DateCreated = entity.DateCreated,
            DateUpdated = entity.DateUpdated,
            DateDeleted = entity.DateDeleted
        };
    }

    public static IEnumerable<UserProfileDocument> Map(IEnumerable<UserProfile> entities)
    {
        return entities
            .Select(Map)
            .Where(x => x is not null)
            .Cast<UserProfileDocument>();
    }
}