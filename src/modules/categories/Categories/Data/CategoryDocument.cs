using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Modules.Categories.Data;

[BsonCollection("transaction_categories")]
public class CategoryDocument : MongoDocument
{
    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = "";

    [BsonElement("sort_order")]
    public int SortOrder { get; set; }

    [BsonElement("parent_id")]
    public ObjectId? ParentId { get; set; }

    [BsonElement("sub_categories")]
    public List<CategoryDocument> SubCategories { get; set; }

    [BsonElement("monthly_totals")]
    public List<MonthlyTotal> MonthlyTotals { get; set; }

    [BsonElement("merchant_totals")]
    public List<MerchantTotal> MerchantTotals { get; set; }

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

    public CategoryDocument()
    {
        Id = ObjectId.GenerateNewId();
        MonthlyTotals = [];
        MerchantTotals = [];
        SubCategories = [];
    }

    public static CategoryDocument New(
        string userId,
        string name,
        string description,
        int sortOrder = 99)
    {
        return new CategoryDocument
        {
            UserId = userId,
            Name = name,
            Description = description,
            SortOrder = sortOrder,
            IsDeleted = false,
            DateCreated = DateTime.UtcNow,
        };
    }

    public void AddSubCategory(string name, string description, int sortOrder)
    {
        var subCategory = CategoryDocument.New(UserId, name, description, sortOrder);
        subCategory.ParentId = this.Id;

        SubCategories.Add(subCategory);
    }

    public sealed class MonthlyTotal
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Total { get; set; }
        public int TransactionCount { get; set; }
    }

    public sealed class MerchantTotal
    {
        public ObjectId MerchantId { get; set; }
        public decimal Total { get; set; }
        public int TransactionCount { get; set; }
    }
}