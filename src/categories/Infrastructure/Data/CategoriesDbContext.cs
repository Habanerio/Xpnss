using Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Shared.Data;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Categories.Infrastructure.Data;

public class CategoriesDbContext : XpnssDbContext
{
    //public CategoriesDbContext(IOptions<MongoDbSettings> options) : base(options)
    //{
    //    Configure();
    //}

    public CategoriesDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        Configure();
    }

    protected override void Configure()
    {
        // Category Document
        var uniqueUserCategoryIdIndex = new CreateIndexModel<CategoryDocument>(
            Builders<CategoryDocument>.IndexKeys
                .Ascending(a => a.UserId)
                .Ascending(a => a.Id),
            new CreateIndexOptions { Unique = true }
        );

        Collection<CategoryDocument>().Indexes.CreateOne(uniqueUserCategoryIdIndex);
    }
}