using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Infrastructure;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data;

public class AccountsDbContext : XpnssDbContext //<AccountDocument>
{
    protected IMongoCollection<AccountDocument> Accounts => base.Collection<AccountDocument>();

    protected IMongoCollection<AdjustmentDocument> Adjustments => base.Collection<AdjustmentDocument>();

    protected IMongoCollection<AccountMonthlyTotalDocument> MonthlyTotals => base.Collection<AccountMonthlyTotalDocument>();

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
            new CreateIndexOptions { Unique = true }
            );

        Accounts.Indexes.CreateMany(new[] { uniqueUserAccountIdIndex });

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

        Adjustments.Indexes.CreateMany(indexes);

        // AccountMonthlyTotal Document
        var accountMonthlyTotalIndexes = new List<CreateIndexModel<AccountMonthlyTotalDocument>>
        {
            new CreateIndexModel<AccountMonthlyTotalDocument>(
                Builders<AccountMonthlyTotalDocument>.IndexKeys.Ascending(a => a.AccountId),
                new CreateIndexOptions { Name = "idx_account_id" }),

            new CreateIndexModel<AccountMonthlyTotalDocument>(
                Builders<AccountMonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.AccountId)
                    .Ascending(a => a.Year)
                    .Ascending(a => a.Month),
                new CreateIndexOptions { Name = "idx_account_id_year_month" }),

            new CreateIndexModel<AccountMonthlyTotalDocument>(
                Builders<AccountMonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.UserId),
                new CreateIndexOptions { Name = "idx_user_id" }),

            new CreateIndexModel<AccountMonthlyTotalDocument>(
                Builders<AccountMonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.Year)
                    .Ascending(a => a.Month),
                new CreateIndexOptions { Name = "idx_year_month" })
        };

        MonthlyTotals.Indexes.CreateMany(accountMonthlyTotalIndexes);
    }
}