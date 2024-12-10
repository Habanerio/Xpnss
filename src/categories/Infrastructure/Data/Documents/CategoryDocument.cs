using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Shared.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;

[BsonCollection("transactions_categories")]
public class CategoryDocument : MongoDocument
{
    [BsonElement("user_id")]
    public ObjectId UserId { get; set; }

    [BsonElement("category_name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("category_type")]
    [BsonRepresentation(BsonType.String)]
    public CategoryGroupEnums.CategoryKeys CategoryType { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("sort_order")]
    public int SortOrder { get; set; }

    [BsonElement("sub_categories")]
    public List<SubCategoryDocument> SubCategories { get; set; } = [];

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
}

public class SubCategoryDocument : MongoDocument
{
    [BsonElement("user_id")]
    public ObjectId UserId { get; set; }

    [BsonElement("subCategory_Name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("subCategory_type")]
    [BsonRepresentation(BsonType.String)]
    public CategoryGroupEnums.CategoryKeys CategoryType { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("parent_id")]
    public ObjectId ParentId { get; set; }

    [BsonElement("sort_order")]
    public int SortOrder { get; set; }

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
}