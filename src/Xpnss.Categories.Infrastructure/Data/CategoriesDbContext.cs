using Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Infrastructure;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Categories.Infrastructure.Data;

public class CategoriesDbContext : XpnssDbContext
{
    protected IMongoCollection<CategoryDocument> Categories => Collection<CategoryDocument>();

    protected IMongoCollection<CategoryMonthlyTotalDocument> MonthlyTotals => Collection<CategoryMonthlyTotalDocument>();

    //public CategoriesDbContext(IOptions<MongoDbSettings> options) : base(options)
    //{
    //    Configure();
    //}

    public CategoriesDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        Configure();
    }

    protected override void Configure()
    {
        // Category Document
        var uniqueUserCategoryIdIndex = new CreateIndexModel<CategoryDocument>(
            Builders<CategoryDocument>.IndexKeys
                .Ascending(a => a.UserId)
                .Ascending(a => a.Id),
            new CreateIndexOptions { Unique = true }
        );

        Categories.Indexes.CreateOne(uniqueUserCategoryIdIndex);

        // CategoryMonthlyTotal Document
        var categoryMonthlyTotalIndexes = new List<CreateIndexModel<CategoryMonthlyTotalDocument>>
        {
            new CreateIndexModel<CategoryMonthlyTotalDocument>(
                Builders<CategoryMonthlyTotalDocument>.IndexKeys.Ascending(a =>
                    a.CategoryId),
                new CreateIndexOptions { Name = "idx_category_id" }),

            new CreateIndexModel<CategoryMonthlyTotalDocument>(
                Builders<CategoryMonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.CategoryId)
                    .Ascending(a => a.Year)
                    .Ascending(a => a.Month),
                new CreateIndexOptions { Name = "idx_category_id_year_month" }),

            new CreateIndexModel<CategoryMonthlyTotalDocument>(
                Builders<CategoryMonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.UserId),
                new CreateIndexOptions { Name = "idx_user_id" }),

            new CreateIndexModel<CategoryMonthlyTotalDocument>(
                Builders<CategoryMonthlyTotalDocument>.IndexKeys
                    .Ascending(a => a.Year)
                    .Ascending(a => a.Month),
                new CreateIndexOptions { Name = "idx_year_month" })
        };

        MonthlyTotals.Indexes.CreateMany(categoryMonthlyTotalIndexes);
    }
}