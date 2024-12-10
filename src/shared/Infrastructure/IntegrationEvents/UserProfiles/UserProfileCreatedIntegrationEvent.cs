namespace Habanerio.Xpnss.Infrastructure.IntegrationEvents.UserProfiles;

public record UserProfileCreatedIntegrationEvent : IntegrationEvent
{
    public UserProfileCreatedIntegrationEvent(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userId));

        UserId = userId;
    }

    public string UserId { get; }
}