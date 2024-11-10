using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Infrastructure.Documents;

/// <summary>
/// Used to keep track of changes, such as Balance, Credit Limit, Interest Rate Adjustments.
/// Not for updates such as Name, Description, or Display Color changes.
/// </summary>
[BsonCollection("money_accounts_change_history")]
public sealed class ChangeHistory : MongoDocument
{
    [BsonElement("account_id")]
    public string AccountId { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; } = "";

    [BsonElement("old_value")]
    public string? OldValues { get; set; } = null;

    [BsonElement("new_value")]
    public string NewValues { get; set; } = "";

    [BsonElement("property")]
    public string Property { get; set; } = "";

    [BsonElement("reason")]
    public string Reason { get; set; } = "";

    [BsonElement("date_changed")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime DateChanged { get; set; }
}