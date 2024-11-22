using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Infrastructure.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;

/// <summary>
/// Represents the total creditLimit of money added and removed in a month for a specific Account.
/// </summary>
[BsonCollection("accounts_monthly_totals")]
public sealed class AccountMonthlyTotalDocument : BaseMonthlyTotalDocument
{
    [BsonElement("account_id")]
    public ObjectId AccountId { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; } = "";
}