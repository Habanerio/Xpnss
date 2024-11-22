using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Infrastructure.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;

/// <summary>
/// Represents the total amount of money added and removed in a month for a specific category.
/// </summary>
[BsonCollection("categories_monthly_totals")]
public sealed class CategoryMonthlyTotalDocument : BaseMonthlyTotalDocument
{
    [BsonElement("category_id")]
    public ObjectId CategoryId { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; }
}