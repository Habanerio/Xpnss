using Habanerio.Core.Dbs.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Infrastructure.Documents;

/// <summary>
/// Represents the amount of money added and removed in a month.
/// </summary>
public abstract class BaseMonthlyTotalDocument : MongoDocument
{
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
}