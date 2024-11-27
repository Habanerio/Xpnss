using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public sealed record MonthlyTotalId : EntityObjectId
{
    private MonthlyTotalId()
    {
        SetValue(ObjectId.Empty.ToString());
    }

    public MonthlyTotalId(string? id)
    {
        SetValue(id ?? string.Empty);
    }

    public MonthlyTotalId(ObjectId id)
    {
        SetValue(id.ToString());
    }


    public new static MonthlyTotalId New => new(ObjectId.GenerateNewId().ToString());

    public new static MonthlyTotalId Empty => new();

    public static bool IsEmpty(MonthlyTotalId id) => id.Equals(Empty);


    public static implicit operator string(MonthlyTotalId id) => id.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}