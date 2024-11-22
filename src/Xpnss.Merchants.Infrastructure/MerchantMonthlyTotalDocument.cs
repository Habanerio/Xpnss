using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Infrastructure.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Merchants.Infrastructure;

/// <summary>
/// Represents the total amount of money added and removed in a month for a specific category.
/// </summary>
[BsonCollection("merchants_monthly_totals")]
public sealed class MerchantMonthlyTotalDocument : BaseMonthlyTotalDocument
{
    [BsonElement("merchant_id")]
    public ObjectId MerchantId { get; }

    [BsonElement("user_id")]
    public string UserId { get; }

    public MerchantMonthlyTotalDocument(ObjectId merchantId, string userId)
    {
        MerchantId = merchantId;
        UserId = userId;
    }
}