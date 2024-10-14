using Habanerio.Core.Dbs.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Modules.Transactions.Data;

public class TransactionsDbContext : MongoDbContext<TransactionDocument>
{
    public TransactionsDbContext(IOptions<MongoDbSettings> options) : base(options)
    {
        CreateIndex();
    }

    private void CreateIndex()
    {
        var indexKeysDefinition = Builders<TransactionDocument>.IndexKeys
            .Ascending(a => a.UserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<TransactionDocument>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        Collection().Indexes.CreateOne(createIndexModel);
    }
}