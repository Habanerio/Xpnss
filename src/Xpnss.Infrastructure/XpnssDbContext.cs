using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Infrastructure.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Infrastructure;

public class XpnssDbContext : MongoDbContext
{
    public IMongoCollection<AccountDocument> Accounts =>
        Collection<AccountDocument>();

    public IMongoCollection<CategoryDocument> Categories =>
        Collection<CategoryDocument>();

    public IMongoCollection<MerchantDocument> Merchants =>
        Collection<MerchantDocument>();

    public IMongoCollection<TransactionDocument> Transactions =>
        Collection<TransactionDocument>();

    public XpnssDbContext(IOptions<MongoDbSettings> options) : base(options)
    {
        ConfigureAccounts();
        ConfigureCategories();
        ConfigureMerchants();
        ConfigureTransactions();
    }

    private void ConfigureAccounts()
    {
        var indexKeysDefinition = Builders<AccountDocument>.IndexKeys
            .Ascending(a => a.UserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<AccountDocument>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        Accounts.Indexes.CreateOne(createIndexModel);
    }

    private void ConfigureCategories()
    {
        var indexKeysDefinition = Builders<CategoryDocument>.IndexKeys
            .Ascending(a => a.UserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<CategoryDocument>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        Categories.Indexes.CreateOne(createIndexModel);
    }

    private void ConfigureMerchants()
    {
        var indexKeysDefinition = Builders<MerchantDocument>.IndexKeys
            .Ascending(a => a.UserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<MerchantDocument>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        Merchants.Indexes.CreateOne(createIndexModel);
    }

    private void ConfigureTransactions()
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