using Habanerio.Core.Dbs.MongoDb.Interfaces;
using MongoDB.Driver;

namespace Habanerio.Core.Dbs.MongoDb;

public sealed class MongoDbClient : IMongoDbClient
{
    public IMongoDatabase Database { get; }

    public MongoDbClient(MongoDbSettings options)
    {
        var client = new MongoClient(options.ConnectionString);
        Database = client.GetDatabase(options.DatabaseName);
    }

    public MongoDbClient(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        Database = client.GetDatabase(databaseName);
    }
}