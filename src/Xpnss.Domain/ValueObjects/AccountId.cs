using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public sealed record AccountId : EntityObjectId
{
    public AccountId(string accountId) : base(accountId)
    { }

    public AccountId(ObjectId accountId) : base(accountId)
    { }

    public static AccountId New => new AccountId(ObjectId.GenerateNewId().ToString());

    public static implicit operator string(AccountId accountId) => accountId.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}