using MongoDB.Bson;

namespace Habanerio.Xpnss.Shared.ValueObjects;

public sealed record PayerPayeeId : EntityObjectId
{

    public PayerPayeeId(ObjectId? id) : this(id?.ToString())
    { }

    public PayerPayeeId(string? id)
    {
        SetValue(id ?? string.Empty);
    }

    public new static PayerPayeeId New => new(ObjectId.GenerateNewId());

    public new static PayerPayeeId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(PayerPayeeId id) => id.Equals(Empty);


    public static implicit operator string(PayerPayeeId id) => IsEmpty(id) ? string.Empty : id.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}