using MongoDB.Bson;

namespace Habanerio.Xpnss.Shared.ValueObjects;

/// <summary>
/// Represents a Transaction's Reference Transaction Id,
/// the Id of an existing transaction within the system
/// </summary>
public sealed record RefTransactionId : EntityObjectId
{
    public RefTransactionId(ObjectId? refTransactionId) :
        this(refTransactionId?.ToString())
    { }

    public RefTransactionId(string? refTransactionId)
    {
        SetValue(refTransactionId ?? string.Empty);
    }

    public new static RefTransactionId New => new(ObjectId.GenerateNewId());

    public new static RefTransactionId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(RefTransactionId refTransactionId) =>
        refTransactionId.Equals(Empty);


    public static implicit operator string(RefTransactionId refTransactionId) =>
        refTransactionId.Value;
}