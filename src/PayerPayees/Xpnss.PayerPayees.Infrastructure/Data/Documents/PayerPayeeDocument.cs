using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Documents;

/// <summary>
/// Represents a business or individual that is either paid or pays money.
/// Could be the name of a store, a person, or a company.
/// </summary>
[BsonCollection("payerpayees")]
public sealed class PayerPayeeDocument : MongoDocument
{
    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("locations")]
    public string Location { get; set; }

    public PayerPayeeDocument(string id, string userId, string name, string location)
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