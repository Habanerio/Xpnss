using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public sealed record AccountId : EntityObjectId
{
    public AccountId(string accountId) : base(accountId) { }

    public AccountId(ObjectId accountId) : base(accountId) { }


    public new static AccountId New => new(ObjectId.GenerateNewId());

    public new static AccountId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(AccountId accountId) => accountId.Equals(Empty);


    public static implicit operator string(AccountId accountId) => accountId.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}