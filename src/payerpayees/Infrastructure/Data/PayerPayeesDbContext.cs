using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Shared.Data;
using MongoDB.Driver;

namespace Habanerio.Xpnss.PayerPayees.Infrastructure.Data;

public class PayerPayeesDbContext : XpnssDbContext
{
    //public PayerPayeesDbContext(IOptions<MongoDbSettings> options) : base(options)
    //{
    //    Configure();
    //}

    public PayerPayeesDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        Configure();
    }

    protected override void Configure()
    {
        var uniqueUserPayerPayeeIdIndex = new CreateIndexModel<PayerPayeeDocument>(
            Builders<PayerPayeeDocument>.IndexKeys
                .Ascending(a => a.UserId)
                .Ascending(a => a.Id),
            new CreateIndexOptions { Unique = true });

        Collection<PayerPayeeDocument>().Indexes.CreateOne(uniqueUserPayerPayeeIdIndex);
    }
}