using Habanerio.Core.Dbs.MongoDb.Attributes;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Infrastructure.Documents;

[BsonCollection("merchants_totals")]
public sealed class MerchantTotals
{
    public ObjectId MerchantId { get; set; }
    public decimal Total { get; set; }
    public int TransactionCount { get; set; }
}