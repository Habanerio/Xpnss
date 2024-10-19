using Habanerio.Core.Dbs.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Modules.Categories.Data;

public class CategoriesDbContext : MongoDbContext<CategoryDocument>
{
    public CategoriesDbContext(IOptions<MongoDbSettings> options) : base(options)
    {
        CreateIndex();
    }

    private void CreateIndex()
    {
        var indexKeysDefinition = Builders<CategoryDocument>.IndexKeys
            .Ascending(a => a.UserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<CategoryDocument>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        Collection().Indexes.CreateOne(createIndexModel);
    }
}