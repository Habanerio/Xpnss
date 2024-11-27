namespace Habanerio.Xpnss.Infrastructure.IntegrationEvents.Accounts;

public record AccountDeletedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; }

    public string AccountId { get; }

    public DateTime DateOfDeletionUtc { get; }

    public AccountDeletedIntegrationEvent(string userId, string accountId, DateTime dateOfDeletionUtc)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userId));

        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(accountId));

        UserId = userId;
        AccountId = accountId;
        DateOfDeletionUtc = dateOfDeletionUtc;
    }
}