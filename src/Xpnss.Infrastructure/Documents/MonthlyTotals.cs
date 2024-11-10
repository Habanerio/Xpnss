using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Infrastructure.Documents;

/// <summary>
/// Represents the amount of money added and removed in a month.
/// </summary>
public abstract class MonthlyTotal : MongoDocument
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

/// <summary>
/// Represents the total amount of money added and removed in a month for a specific account.
/// </summary>
[BsonCollection("money_accounts_monthly_totals")]
public sealed class MonthlyAccountTotal : MonthlyTotal
{
    [BsonElement("account_id")]
    public string AccountId { get; init; }

    [BsonElement("user_id")]
    public string UserId { get; set; }
}

/// <summary>
/// Represents the total amount of money added and removed in a month for a specific category.
/// </summary>
[BsonCollection("money_categories_monthly_totals")]
public sealed class MonthlyCategoryTotal : MonthlyTotal
{
    [BsonElement("category_id")]
    public string CategoryId { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; }
}