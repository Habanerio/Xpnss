using Habanerio.Xpnss.Infrastructure;
using Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Documents;
using MongoDB.Driver;

namespace Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data;

public class MonthlyTotalsDbContext : XpnssDbContext
{
    protected IMongoCollection<MonthlyTotalDocument> Collection => Collection<MonthlyTotalDocument>();

    public MonthlyTotalsDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        Configure();
    }

    //public MonthlyTotalsDbContext(IOptions<MongoDbSettings> options) : base(options)
    //{
    //    Configure();
    //}

    protected override void Configure()
    {
        var categoryMonthlyTotalIndexes = new List<CreateIndexModel<MonthlyTotalDocument>>
        {
            new CreateIndexModel<MonthlyTotalDocument>(
                Builders<MonthlyTotalDocument>.IndexKeys.Ascending(a =>
                    a.EntityId),
                new CreateIndexOptions { Name = "idx_entity_id" }),

            new CreateIndexModel<MonthlyTotalDocument>(
                Builders<MonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.EntityId)
                    .Ascending(a => a.Year)
                    .Ascending(a => a.Month),
                new CreateIndexOptions { Name = "idx_entity_id_year_month" }),

            new CreateIndexModel<MonthlyTotalDocument>(
                Builders<MonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.UserId),
                new CreateIndexOptions { Name = "idx_user_id" }),

            new CreateIndexModel<MonthlyTotalDocument>(
                Builders<MonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.Year)
                    .Ascending(a => a.Month),
                new CreateIndexOptions { Name = "idx_year_month" }),

            new CreateIndexModel<MonthlyTotalDocument>(
                Builders<MonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.UserId)
                    .Ascending(a=>a.EntityType)
                    .Ascending(a => a.Year)
                    .Ascending(a => a.Month),
                new CreateIndexOptions { Name = "idx_userid_entitytype_year_month" })
        };

        Collection.Indexes.CreateMany(categoryMonthlyTotalIndexes);
    }
}