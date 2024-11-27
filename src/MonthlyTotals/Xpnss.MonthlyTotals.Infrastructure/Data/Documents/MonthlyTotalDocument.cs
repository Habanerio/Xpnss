using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Infrastructure.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Documents;

/// <summary>
/// Represents the total amount of money added and removed in a month for a specific category.
/// </summary>
[BsonCollection("monthly_totals")]
public sealed class MonthlyTotalDocument : BaseMonthlyTotalDocument
{
    [BsonElement("entity_id")]
    public string EntityId { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("entity_type")]
    [BsonRepresentation(BsonType.String)]
    public EntityTypes.Keys EntityType { get; set; }
}