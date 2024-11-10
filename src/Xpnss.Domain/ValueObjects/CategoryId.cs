using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public sealed record CategoryId : EntityObjectId
{
    private CategoryId() : this(ObjectId.Empty.ToString())
    { }

    public CategoryId(ObjectId? categoryId) : this(categoryId?.ToString())
    { }

    public CategoryId(string? categoryId)
    {
        SetValue(categoryId ?? string.Empty);
    }

    public static CategoryId New => new CategoryId(ObjectId.GenerateNewId().ToString());

    public static CategoryId Empty => new CategoryId();

    public static implicit operator string(CategoryId userId) => userId.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}