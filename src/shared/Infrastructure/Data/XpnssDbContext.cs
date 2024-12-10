using Habanerio.Core.Dbs.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Infrastructure.Data;

public abstract class XpnssDbContext : MongoDbContext
{
    public XpnssDbContext(IOptions<MongoDbSettings> options) : base(options)
    {
        Configure();
    }

    public XpnssDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        Configure();
    }

    protected abstract void Configure();
}

public abstract class XpnssDbContext<TDocument> : MongoDbContext<TDocument> where TDocument : MongoDocument
{
    public XpnssDbContext(IOptions<MongoDbSettings> options) : base(options)
    {
        Configure();
    }

    protected abstract void Configure();
}