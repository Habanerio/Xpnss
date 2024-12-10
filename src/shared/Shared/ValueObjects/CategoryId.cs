using MongoDB.Bson;

namespace Habanerio.Xpnss.Shared.ValueObjects;

//NOTE: Should I make the CategoryId properties in other entities nullable?
//      Or allow nullable values to be passed into the CategoryId()?
public record CategoryId : EntityObjectId
{
    public CategoryId(ObjectId? id) : this(id?.ToString())
    { }

    public CategoryId(string? id)
    {
        SetValue(id ?? string.Empty);
    }

    public new static CategoryId New => new(ObjectId.GenerateNewId());

    public new static CategoryId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(CategoryId id) => id.Equals(Empty);


    public static implicit operator string(CategoryId id) => id.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}