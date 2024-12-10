using MongoDB.Bson;

namespace Habanerio.Xpnss.Shared.ValueObjects;

public record SubCategoryId : CategoryId
{
    public SubCategoryId(ObjectId? id) : base(id)
    { }

    public SubCategoryId(string? id) : base(id)
    { }

    public new static SubCategoryId New => new(ObjectId.GenerateNewId());

    public new static SubCategoryId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(SubCategoryId id) => id.Equals(Empty);


    public static implicit operator string(SubCategoryId id) => id.Value;
}