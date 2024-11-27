using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public sealed record PayerPayeeId : EntityObjectId
{
    private PayerPayeeId()
    {
        SetValue(ObjectId.Empty.ToString());
    }

    public PayerPayeeId(string? id)
    {
        SetValue(id ?? string.Empty);
    }

    public PayerPayeeId(ObjectId id)
    {
        SetValue(id.ToString());
    }


    public new static PayerPayeeId New => new(ObjectId.GenerateNewId().ToString());

    public new static PayerPayeeId Empty => new();

    public static bool IsEmpty(PayerPayeeId id) => id.Equals(Empty);


    public static implicit operator string(PayerPayeeId id) => id.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}