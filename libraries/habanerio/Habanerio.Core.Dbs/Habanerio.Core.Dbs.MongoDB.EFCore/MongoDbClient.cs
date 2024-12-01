using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Core.DBs.MongoDB.EFCore;

public sealed class MongoDbClient : MongoClient
{
    private const string ERROR_COULD_NOT_GET_OPTIONS = "Could not retrieve the options";
    private const string ERROR_COULD_NOT_GET_CONN_STRING = "The Connection String name cannot be null or empty";
    private const string ERROR_COULD_NOT_GET_DBNAME = "Could not retrieve the database name from the options";

    public IMongoDatabase Database { get; }

    public MongoDbClient(IMongoClient client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        Database = client.GetDatabase(client.Settings.ApplicationName);
    }

    public MongoDbClient(IOptions<MongoDbSettings> options)
    {
        if (options?.Value == null)
            throw new ArgumentException(ERROR_COULD_NOT_GET_OPTIONS, nameof(options));

        if (string.IsNullOrWhiteSpace(options.Value.ConnectionString))
            throw new ArgumentException(ERROR_COULD_NOT_GET_CONN_STRING, nameof(options.Value.ConnectionString));

        if (string.IsNullOrWhiteSpace(options.Value.DatabaseName))
            throw new ArgumentException(ERROR_COULD_NOT_GET_DBNAME, nameof(options.Value.DatabaseName));

        var optionValue = options.Value;

        var connectionString = optionValue.ConnectionString;
        var databaseName = optionValue.DatabaseName;

        var client = new MongoClient(connectionString);
        Database = client.GetDatabase(databaseName);
    }

    public MongoDbClient(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        Database = client.GetDatabase(databaseName);
    }
}