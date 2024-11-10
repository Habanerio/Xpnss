namespace Habanerio.Xpnss.Domain.ChangeHistories;

/// <summary>
/// Used to keep track of changes, such as Balance, Credit Limit, Interest Rate Adjustments.
/// Not for updates such as Name, Description, or Display Color changes.
/// </summary>
public sealed class ChangeHistory
{
    public string AccountId { get; init; }

    public string UserId { get; init; }

    public string? OldValue { get; init; } = null;

    public string NewValue { get; init; }

    public string Property { get; init; }

    public string Reason { get; init; }

    public DateTime DateCreated { get; }

    public DateTime DateChanged { get; init; }

    public ChangeHistory(
        string accountId,
        string userId,
        string property,
        string oldValue,
        string newValue,
        DateTime dateChanged,
        string reason = "")
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException($"{nameof(accountId)} cannot be null or whitespace.", nameof(accountId));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException($"{nameof(userId)} cannot be null or whitespace.", nameof(userId));

        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException($"{nameof(newValue)} cannot be null or whitespace.", nameof(newValue));

        if (string.IsNullOrWhiteSpace(property))
            throw new ArgumentException($"{nameof(property)} cannot be null or whitespace.", nameof(property));

        if (dateChanged.ToUniversalTime() > DateTime.UtcNow)
            throw new ArgumentException($"{nameof(dateChanged)} cannot be in the future.", nameof(dateChanged));

        AccountId = accountId;
        UserId = userId;
        OldValue = oldValue;
        NewValue = newValue;
        Property = property;
        Reason = reason;
        DateChanged = dateChanged;
        DateCreated = DateTime.UtcNow;
    }
}