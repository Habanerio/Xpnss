using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Infrastructure.Documents;

[BsonCollection("merchants")]
public sealed class MerchantDocument : MongoDocument
{
    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("accountName")]
    public string Name { get; set; }

    [BsonElement("locations")]
    public string Location { get; set; }

    public MerchantDocument(string id, string userId, string name, string location)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        Id = ObjectId.Parse(id);
        UserId = userId;
        Name = name;
        Location = location;
    }
}