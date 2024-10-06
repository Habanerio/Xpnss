using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Core.Dbs.MongoDb.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Core.Dbs.MongoDb;

public class MongoDbContext<TDocument> : IMongoDbContext<TDocument> where TDocument : IMongoDocument
{
    private readonly IMongoDatabase _db;

    private const string COULD_NOT_GET_OPTIONS = "Could not retrieve the options";
    private const string COULD_NOT_GET_CONNECTION_STRING = "Could not retrieve the connection string";
    private const string COULD_NOT_GET_DBNAME = "Could not retrieve the database name";

    public MongoDbContext(IMongoDbClient client)
    {
        _db = client?.Database ??
              throw new ArgumentNullException(nameof(client));

        var x = nameof(TDocument);
    }

    public MongoDbContext(string connectionString, string databaseName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException(COULD_NOT_GET_CONNECTION_STRING, nameof(connectionString));

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException(COULD_NOT_GET_DBNAME, nameof(databaseName));

        var client = new MongoClient(connectionString);

        _db = client.GetDatabase(databaseName);
    }

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        if (options?.Value == null)
            throw new ArgumentException(COULD_NOT_GET_OPTIONS, nameof(options));

        if (string.IsNullOrWhiteSpace(options.Value.ConnectionString))
            throw new ArgumentException(COULD_NOT_GET_CONNECTION_STRING, nameof(options));

        if (string.IsNullOrWhiteSpace(options.Value.DatabaseName))
            throw new ArgumentException(COULD_NOT_GET_DBNAME, nameof(options));

        var optionValue = options.Value;

        var connection = optionValue.ConnectionString;
        var dbName = optionValue.DatabaseName;

        var client = new MongoClient(connection);

        _db = client.GetDatabase(dbName);
    }

    public IMongoCollection<TDocument> Collection()
    {
        var collection = _db.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));

        return collection;
    }

    private static string GetCollectionName(Type documentType)
    {
        var name = (documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true)
            .FirstOrDefault() as BsonCollectionAttribute)?
            .CollectionName ??
            string.Empty;

        return name;
    }
}
