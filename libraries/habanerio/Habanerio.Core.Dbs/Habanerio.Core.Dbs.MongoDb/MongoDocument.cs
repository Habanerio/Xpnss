using Habanerio.Core.Dbs.MongoDb.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Core.Dbs.MongoDb;

public abstract class MongoDocument : IMongoDocument
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
}