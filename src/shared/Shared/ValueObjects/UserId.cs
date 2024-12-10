using MongoDB.Bson;

namespace Habanerio.Xpnss.Shared.ValueObjects;

public sealed record UserId : EntityObjectId
{
    public UserId(string id) : base(id) { }

    public UserId(ObjectId id) : base(id) { }


    public new static UserId New => new(ObjectId.GenerateNewId());

    public new static UserId Empty => new(ObjectId.Empty);


    public static implicit operator string(UserId id) => id.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}