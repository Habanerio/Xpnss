using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Core.Dbs.MongoDb.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Core.Dbs.MongoDb;

public class MongoDbContext : MongoDbContextBase
{
    public MongoDbContext(IMongoDatabase database) : base(database)
    { }

    public MongoDbContext(string connectionString, string databaseName) : base(connectionString, databaseName)
    { }

    public MongoDbContext(IOptions<MongoDbSettings> options) : base(options)
    { }

    public MongoDbContext(IMongoClient client, string databaseName) : base(client, databaseName)
    { }
}

/// <summary>
/// When you want a single DbContext per Document.
/// </summary>
/// <typeparam name="TDocument"></typeparam>
public class MongoDbContext<TDocument> : MongoDbContextBase, IMongoDbContext<TDocument> where TDocument : IMongoDocument
{
    public IMongoCollection<TDocument> Collection => Collection<TDocument>();

    public MongoDbContext(IMongoDatabase database) : base(database)
    { }

    public MongoDbContext(string connectionString, string databaseName) : base(connectionString, databaseName)
    { }

    public MongoDbContext(IOptions<MongoDbSettings> options) : base(options)
    { }

    public MongoDbContext(IMongoClient client, string databaseName) : base(client, databaseName)
    { }
}

public abstract class MongoDbContextBase : IMongoDbContext
{
    protected readonly IMongoDatabase MongoDatabase;

    private const string COULD_NOT_GET_OPTIONS = "Could not retrieve the options";
    private const string COULD_NOT_GET_CONNECTION_STRING = "Could not retrieve the connection string";
    private const string COULD_NOT_GET_DBNAME = "Could not retrieve the database name";

    protected MongoDbContextBase(IMongoDatabase database)
    {
        MongoDatabase = database ?? throw new ArgumentNullException(nameof(database));
    }

    protected MongoDbContextBase(string connectionString, string databaseName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException(COULD_NOT_GET_CONNECTION_STRING, nameof(connectionString));

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException(COULD_NOT_GET_DBNAME, nameof(databaseName));

        var client = new MongoClient(connectionString);

        MongoDatabase = client.GetDatabase(databaseName);
    }

    protected MongoDbContextBase(IOptions<MongoDbSettings> options)
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

        MongoDatabase = client.GetDatabase(dbName);
    }

    protected MongoDbContextBase(IMongoClient client, string databaseName)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException(COULD_NOT_GET_DBNAME, nameof(databaseName));

        MongoDatabase = client.GetDatabase(databaseName);
    }

    public IMongoCollection<TDocument> Collection<TDocument>()
    {
        var collection = MongoDatabase.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));

        return collection;
    }

    protected static string GetCollectionName(Type documentType)
    {
        var name = (documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true)
                       .FirstOrDefault() as BsonCollectionAttribute)?
                   .CollectionName ??
                   string.Empty;

        return name;
    }
}