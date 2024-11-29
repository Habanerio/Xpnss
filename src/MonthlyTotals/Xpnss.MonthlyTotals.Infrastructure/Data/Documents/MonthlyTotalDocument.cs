using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Domain.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Documents;

/// <summary>
/// Represents the total amount of money added and removed in a month for a specific category.
/// </summary>
[BsonCollection("monthly_totals")]
public sealed class MonthlyTotalDocument : MongoDocument
{
    [BsonElement("entity_id")]
    public ObjectId? EntityId { get; set; }

    [BsonElement("user_id")]
    public ObjectId UserId { get; set; }

    [BsonElement("entity_type")]
    [BsonRepresentation(BsonType.String)]
    public EntityTypes.Keys EntityType { get; set; }

    [BsonElement("year")]
    public int Year { get; set; }

    [BsonElement("month")]
    public int Month { get; set; }

    /// <summary>
    /// Total amount of money added.
    /// </summary>
    [BsonElement("credit_total")]
    public decimal CreditTotal { get; set; }

    /// <summary>
    /// Total number of credits added.
    /// </summary>
    [BsonElement("credit_count")]
    public int CreditCount { get; set; }

    /// <summary>
    /// Total amount of money removed.
    /// </summary>
    [BsonElement("debit_total")]
    public decimal DebitTotal { get; set; }

    /// <summary>
    /// Total number of debits removed.
    /// </summary>
    [BsonElement("debit_count")]
    public int DebitCount { get; set; }

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