using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Infrastructure;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data;

public class AccountsDbContext : XpnssDbContext //<AccountDocument>
{
    //public AccountsDbContext(IOptions<MongoDbSettings> options) : base(options)
    //{
    //    Configure();
    //}

    public AccountsDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        Configure();
    }

    protected override void Configure()
    {
        // Account Document
        var uniqueUserAccountIdIndex = new CreateIndexModel<AccountDocument>(
            Builders<AccountDocument>.IndexKeys
                .Ascending(a => a.UserId)
                .Ascending(a => a.Id),
            new CreateIndexOptions { Unique = true });

        Collection<AccountDocument>().Indexes.CreateMany(new[] { uniqueUserAccountIdIndex });

        // Adjustment Document
        var indexes = new List<CreateIndexModel<AdjustmentDocument>>
        {
            new CreateIndexModel<AdjustmentDocument>(
                Builders<AdjustmentDocument>.IndexKeys.Ascending(a => a.AccountId),
                new CreateIndexOptions { Name = "idx_account_id" }),

            new CreateIndexModel<AdjustmentDocument>(
                Builders<AdjustmentDocument>.IndexKeys
                    .Ascending(a => a.AccountId)
                    .Ascending(a => a.DateChanged),
                new CreateIndexOptions { Name = "idx_account_id_date_changed" }),

            new CreateIndexModel<AdjustmentDocument>(
                Builders<AdjustmentDocument>.IndexKeys.Ascending(a => a.UserId),
                new CreateIndexOptions { Name = "idx_user_id" }),

            new CreateIndexModel<AdjustmentDocument>(
                Builders<AdjustmentDocument>.IndexKeys.Ascending(a => a.Property),
                new CreateIndexOptions { Name = "idx_property" }),

            new CreateIndexModel<AdjustmentDocument>(
                Builders<AdjustmentDocument>.IndexKeys.Ascending(a => a.DateChanged),
                new CreateIndexOptions { Name = "idx_date_changed" })
        };

        Collection<AdjustmentDocument>().Indexes.CreateMany(indexes);
    }
}