using Habanerio.Xpnss.Infrastructure;
using Habanerio.Xpnss.Transactions.Infrastructure.Data.Documents;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Transactions.Infrastructure.Data;

public class TransactionsDbContext : XpnssDbContext
{
    protected IMongoCollection<TransactionDocument> Transactions => Collection<TransactionDocument>();

    //public TransactionsDbContext(IOptions<MongoDbSettings> options) : base(options)
    //{
    //    Configure();
    //}

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

        Transactions.Indexes.CreateOne(createIndexModel);
    }
}