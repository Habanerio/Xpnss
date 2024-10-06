using MongoDB.Driver;

namespace Habanerio.Core.Dbs.MongoDb.Interfaces;

public interface IMongoDbClient
{
    IMongoDatabase Database { get; }
}