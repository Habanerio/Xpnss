using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Interfaces;
using Microsoft.Extensions.Options;

namespace Habanerio.Xpnss.Infrastructure.Data;

public class XpnssDbRepository<TDocument> :
    MongoDbRepository<TDocument> where TDocument :
    IMongoDocument
{
    public XpnssDbRepository(IOptions<MongoDbSettings> options) : base(options)
    {
    }

    public XpnssDbRepository(string connectionString, string databaseName) : base(connectionString, databaseName)
    {
    }

    public XpnssDbRepository(MongoDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// Wrapper around the base AddDocumentAsync method that returns a boolean indicating if the document was saved.
    /// If the document's Id is equal to ObjectId.Empty before saving, and is not after saving, then the document was saved.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public new async Task<bool> AddDocumentAsync(TDocument document, CancellationToken cancellationToken = default)
    {
        var hasDefaultId = document.Id == default;

        await base.AddDocumentAsync(document, cancellationToken);

        return !hasDefaultId || document.Id != default;
    }
}