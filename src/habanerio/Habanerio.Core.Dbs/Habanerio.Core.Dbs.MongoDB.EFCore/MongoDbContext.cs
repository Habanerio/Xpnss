using Habanerio.Core.DBs.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Habanerio.Core.DBs.MongoDB.EFCore;

public class MongoDbContext : DbContextBase
{
    public MongoDbContext(IOptions<MongoDbSettings> options) : this(GetDbContextOptions(options)) { }

    public MongoDbContext(DbContextOptions options) : base(options) { }


    protected static DbContextOptions GetDbContextOptions(IOptions<MongoDbSettings> options)
    {
        var mongoOptions = options.Value;

        var connectionString = mongoOptions.ConnectionString;
        var databaseName = mongoOptions.DatabaseName;
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();

        dbContextOptionsBuilder.UseMongoDB(connectionString, databaseName);

        if (mongoOptions.EnableSensitiveDataLogging)
            dbContextOptionsBuilder.EnableSensitiveDataLogging();

        if (mongoOptions.EnableDetailedErrors)
            dbContextOptionsBuilder.EnableDetailedErrors();

        return dbContextOptionsBuilder.Options;
    }
}
