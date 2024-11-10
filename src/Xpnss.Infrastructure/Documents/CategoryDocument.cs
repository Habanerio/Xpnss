using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Infrastructure.Documents;

[BsonCollection("transactions_categories")]
public class CategoryDocument : MongoDocument
{
    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("accountName")]
    public string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("sort_order")]
    public int SortOrder { get; set; }

    [BsonElement("parent_id")]
    public string? ParentId { get; set; }

    [BsonElement("sub_categories")]
    public List<CategoryDocument> SubCategories { get; set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }

    [BsonElement("date_created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateCreated { get; set; }

    [BsonElement("date_updated")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateUpdated { get; set; }

    [BsonElement("date_deleted")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateDeleted { get; set; }


    public CategoryDocument(
        ObjectId categoryId,
        string userId,
        string name,
        string description,
        int sortOrder,
        List<CategoryDocument> subCategories,
        ObjectId? parentId = null)
    {
        Id = categoryId;
        UserId = userId;
        Name = name;
        Description = description;
        SubCategories = subCategories;
        ParentId = parentId.ToString();
        SortOrder = sortOrder;
        IsDeleted = false;
        DateCreated = DateTime.UtcNow;
    }

    public static CategoryDocument New(
        string userId,
        string name,
        List<CategoryDocument> subCategories,
        string description,
        int sortOrder = 99)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        return new CategoryDocument(ObjectId.GenerateNewId(), userId, name, description, sortOrder, subCategories);
    }

    public static CategoryDocument NewSub(
        string userId,
        string name,
        List<CategoryDocument> subCategories,
        string description,
        string parentId,
        int sortOrder = 99)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        if (string.IsNullOrWhiteSpace(parentId))
            throw new ArgumentNullException(nameof(parentId));

        return new CategoryDocument(ObjectId.GenerateNewId(), userId, name, description, sortOrder, subCategories, ObjectId.Parse(parentId));
    }

    public void AddSubCategory(string name, string description, int sortOrder)
    {
        var subCategory = NewSub(UserId, name, [], description, Id.ToString(), sortOrder);

        SubCategories.Add(subCategory);
    }
}