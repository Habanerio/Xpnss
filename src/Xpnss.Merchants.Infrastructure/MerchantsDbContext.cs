using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Merchants.Infrastructure;

public class MerchantsDbContext : XpnssDbContext
{
    public MerchantsDbContext(IOptions<MongoDbSettings> options) : base(options)
    {
        Configure();
    }

    protected override void Configure()
    {
        var indexKeysDefinition = Builders<MerchantDocument>.IndexKeys
            .Ascending(a => a.UserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<MerchantDocument>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        Collection<MerchantDocument>().Indexes.CreateOne(createIndexModel);
    }
}