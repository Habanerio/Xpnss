using MongoDB.Bson;

namespace Habanerio.Core.Dbs.MongoDb.Interfaces;

/// <summary>
/// A wrapper interface for MongoDB entities that specified that the Id should be of type ObjectId.
/// </summary>
public interface IMongoDocument : IMongoDocument<ObjectId> { }

public interface IMongoDocument<out TId>
{
    TId Id { get; }
}