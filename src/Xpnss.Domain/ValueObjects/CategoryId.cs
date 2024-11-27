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

    public new static CategoryId New => new(ObjectId.GenerateNewId().ToString());

    public new static CategoryId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(CategoryId categoryId) => categoryId.Equals(Empty);


    public static implicit operator string(CategoryId userId) => userId.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}