using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Transactions.Infrastructure;

public class TransactionsDbContext : XpnssDbContext
{
    public TransactionsDbContext(IOptions<MongoDbSettings> options) : base(options)
    {
        Configure();
    }

    protected override void Configure()
    {
        var indexKeysDefinition = Builders<TransactionDocument>.IndexKeys
            .Ascending(a => a.UserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<TransactionDocument>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        Collection<TransactionDocument>().Indexes.CreateOne(createIndexModel);
    }
}