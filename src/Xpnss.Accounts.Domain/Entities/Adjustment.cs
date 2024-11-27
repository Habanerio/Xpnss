using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Accounts.Domain.Entities;

/// <summary>
/// Used to keep track of changes, such as Balance, Credit Limit, Interest Rate Adjustments.
/// Not for updates such as Name, Description, or Display Color changes.
/// </summary>
public sealed class Adjustment : Entity<EntityObjectId>
{
    public AccountId AccountId { get; init; }

    public UserId UserId { get; init; }

    public string Value { get; init; }

    public string Property { get; init; }

    public string Reason { get; init; }

    public DateTime DateCreated { get; }

    public DateTime DateChanged { get; init; }

    private Adjustment(
        EntityObjectId id,
        AccountId accountId,
        UserId userId,
        string property,
        string value,
        DateTime dateChanged,
        string reason = "") : base(id)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Value.Equals(ObjectId.Empty.ToString()))
            throw new ArgumentException($"{nameof(id)} cannot be null or whitespace.", nameof(id));

        if (string.IsNullOrWhiteSpace(accountId) || accountId.Value.Equals(ObjectId.Empty.ToString()))
            throw new ArgumentException($"{nameof(accountId)} cannot be null or whitespace.", nameof(accountId));

        if (string.IsNullOrWhiteSpace(userId) || userId.Value.Equals(ObjectId.Empty.ToString()))
            throw new ArgumentException($"{nameof(userId)} cannot be null or whitespace.", nameof(userId));

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{nameof(value)} cannot be null or whitespace.", nameof(value));

        if (string.IsNullOrWhiteSpace(property))
            throw new ArgumentException($"{nameof(property)} cannot be null or whitespace.", nameof(property));

        //if (dateChanged.ToUniversalTime() > DateTime.UtcNow)
        //    throw new ArgumentException($"{nameof(dateChanged)} cannot be in the future.", nameof(dateChanged));

        AccountId = accountId;
        UserId = userId;
        Value = value;
        Property = property;
        Reason = reason;
        DateChanged = dateChanged;
        DateCreated = DateTime.UtcNow;
    }

    public static Adjustment Load(
        EntityObjectId id,
        AccountId accountId,
        UserId userId,
        string property,
        string value,
        DateTime dateChanged,
        string reason = "")
    {
        return new Adjustment(id, accountId, userId, property, value, dateChanged, reason);
    }

    public static Adjustment New(
        AccountId accountId,
        UserId userId,
        string property,
        string value,
        DateTime dateChanged,
        string reason = "")
    {
        return new Adjustment(EntityObjectId.New, accountId, userId, property, value, dateChanged, reason);
    }
}