using Habanerio.Xpnss.Infrastructure;
using Habanerio.Xpnss.UserProfiles.Infrastructure.Data.Documents;
using MongoDB.Driver;

namespace Habanerio.Xpnss.UserProfiles.Infrastructure.Data;

public class UserProfileSettingsDbContext : XpnssDbContext
{
    //public TransactionsDbContext(IOptions<MongoDbSettings> options) : base(options)
    //{
    //    Configure();
    //}

    public UserProfileSettingsDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        Configure();
    }

    protected override void Configure()
    {
        var indexKeysDefinition = Builders<UserProfileDocument>.IndexKeys
            .Ascending(a => a.ExtUserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<UserProfileDocument>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "ExtUserId_Id_Index" }
        );

        Collection<UserProfileDocument>().Indexes.CreateOne(createIndexModel);
    }
}