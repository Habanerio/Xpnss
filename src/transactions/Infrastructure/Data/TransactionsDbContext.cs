using Habanerio.Xpnss.Shared.Data;
using Habanerio.Xpnss.Transactions.Infrastructure.Data.Documents;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Transactions.Infrastructure.Data;

public class TransactionsDbContext : XpnssDbContext
{
    public TransactionsDbContext(IMongoDatabase mongoDb) : base(mongoDb)
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