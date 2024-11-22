using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public sealed record MonthlyTotalId : EntityObjectId
{
    public MonthlyTotalId(string monthlyTotalId) : base(monthlyTotalId)
    { }

    public MonthlyTotalId(ObjectId monthlyTotalId) : base(monthlyTotalId)
    { }

    public static MonthlyTotalId New => new MonthlyTotalId(ObjectId.GenerateNewId().ToString());

    public static implicit operator string(MonthlyTotalId monthlyTotalId) => monthlyTotalId.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}