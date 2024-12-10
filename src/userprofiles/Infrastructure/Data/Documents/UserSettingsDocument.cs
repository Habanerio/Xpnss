using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.UserProfiles.Infrastructure.Data.Documents;

[BsonCollection("user_settings")]
public class UserSettingsDocument : MongoDocument
{
    [BsonElement("user_id")]
    public ObjectId UserId { get; set; }
}