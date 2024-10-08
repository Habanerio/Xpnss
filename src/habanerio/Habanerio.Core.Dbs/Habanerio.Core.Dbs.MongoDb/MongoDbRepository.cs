using System.Linq.Expressions;
using Habanerio.Core.Dbs.MongoDb.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Core.Dbs.MongoDb;

public abstract class MongoDbRepository<TDocument, TId> :
    IMongoDbRepository<TDocument, TId> where TDocument : IMongoDocument
{
    private readonly IMongoDbContext<TDocument> _context;

    // Best to have your messages in a const for performance reasons
    internal const string EXCEPTION_COLLECTION_NOT_FOUND = "The collection was not found";
    internal const string EXCEPTION_ID_CANT_BE_EMPTY = "The Id(s) cannot be empty";

    protected IMongoCollection<TDocument> Collection;

    protected MongoDbRepository(IOptions<MongoDbSettings> options)
    {
        _context = new MongoDbContext<TDocument>(options);

        PopulateCollection();
    }

    protected MongoDbRepository(string connectionString, string databaseName)
    {
        _context = new MongoDbContext<TDocument>(connectionString, databaseName);

        PopulateCollection();
    }

    protected MongoDbRepository(MongoDbContext<TDocument> dbContext)
    {
        _context = dbContext;
        PopulateCollection();
    }

    public void AddDocument(TDocument document)
    {
        Collection.InsertOne(document);
    }

    public async Task AddDocumentAsync(TDocument document, CancellationToken cancellationToken = default)
    {
        await Collection.InsertOneAsync(document, new InsertOneOptions(), cancellationToken);
    }

    public void AddDocuments(IEnumerable<TDocument> documents)
    {
        if (!documents.TryGetNonEnumeratedCount(out var documentsCount) || documentsCount == 0)
            throw new ArgumentException("The documents cannot be empty", nameof(documents));

        Collection.InsertManyAsync(documents, new InsertManyOptions());
    }

    public async Task AddDocumentsAsync(
        IEnumerable<TDocument> documents,
        CancellationToken cancellationToken = default)
    {
        if (!documents.TryGetNonEnumeratedCount(out var documentsCount) || documentsCount == 0)
            throw new ArgumentException("The documents cannot be empty", nameof(documents));

        await Collection.InsertManyAsync(documents, new InsertManyOptions(), cancellationToken: cancellationToken);
    }


    public Task<IEnumerable<TDocument>> FindAsync(
        Expression<Func<TDocument, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.Where(predicate);

        return FindAsync(filter, cancellationToken);
    }

    public Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> FindAsync(
        Expression<Func<TDocument, bool>> predicate,
        int pageNo,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.Where(predicate);

        return FindAsync(filter, pageNo, pageSize, cancellationToken);
    }


    public async Task<TDocument?> FirstOrDefaultAsync(
        Expression<Func<TDocument, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.Where(predicate);

        var rslts = await FindAsync(filter, cancellationToken);

        return rslts.FirstOrDefault();
    }


    public abstract Task<TDocument> GetAsync(
        TId id,
        CancellationToken cancellationToken = default);

    public abstract Task<IEnumerable<TDocument>> GetAsync(
        IEnumerable<TId> ids,
        CancellationToken cancellationToken = default);


    public Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> ListAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> ListAsync(
        int pageNo,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    public virtual async Task<long> UpdateDocumentAsync(
        TDocument document,
        CancellationToken cancellationToken = default)
    {
        /* if (document is IHasDateTimeOffsetUpdated dateUpdatedDocument)
        {
            dateUpdatedDocument.DateUpdated = DateTimeOffset.UtcNow;
        } */

        var rslt = await Collection
            .ReplaceOneAsync(d => d.Id == document.Id,
                document,
                cancellationToken: cancellationToken);

        return rslt.ModifiedCount;
    }

    #region - Privates -

    private void PopulateCollection()
    {
        Collection = _context.Collection() ?? throw new InvalidOperationException(EXCEPTION_COLLECTION_NOT_FOUND);
    }

    internal async Task<IEnumerable<TDocument>> FindAsync(
        FilterDefinition<TDocument> filter,
        CancellationToken cancellationToken = default)
    {
        if (filter is null)
            throw new ArgumentNullException(nameof(filter));

        var cursor = await Collection.FindAsync(filter, null, cancellationToken);

        var results = await cursor.ToListAsync(cancellationToken);

        return results;
    }

    internal async Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> FindAsync(
        FilterDefinition<TDocument> filter,
        int pageNo = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        if (filter is null)
            throw new ArgumentNullException(nameof(filter));

        if (pageNo < 0)
            throw new ArgumentOutOfRangeException(nameof(pageNo));

        if (pageSize < 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var skip = (pageNo - 1) * pageSize;

        var totalCount = await Collection.CountDocumentsAsync(filter, null, cancellationToken);

        if (totalCount == 0)
            return (Enumerable.Empty<TDocument>(), 0, 0);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var options = new FindOptions<TDocument>
        {
            Skip = skip,
            Limit = pageSize,
            Sort = Builders<TDocument>.Sort.Ascending(doc => doc.Id)
        };

        var cursor = await Collection.FindAsync(filter, options, cancellationToken);

        var results = await cursor.ToListAsync(cancellationToken);

        return (results, totalPages, (int)totalCount);
    }

    #endregion
}

public class MongoDbRepository<TDocument> : MongoDbRepository<TDocument, ObjectId> where TDocument : IMongoDocument
{
    protected MongoDbRepository(IOptions<MongoDbSettings> options) : base(options) { }

    protected MongoDbRepository(string connectionString, string databaseName) : base(connectionString, databaseName) { }

    protected MongoDbRepository(MongoDbContext<TDocument> dbContext) : base(dbContext) { }

    public override async Task<TDocument> GetAsync(
        ObjectId id,
        CancellationToken cancellationToken = default)
    {
        if (id.Equals(ObjectId.Empty))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);

        var results = await Collection.FindAsync(filter, cancellationToken: cancellationToken);

        return await results.SingleOrDefaultAsync(cancellationToken);
    }

    public override async Task<IEnumerable<TDocument>> GetAsync(
        IEnumerable<ObjectId> ids,
        CancellationToken cancellationToken = default)
    {
        if (!ids.TryGetNonEnumeratedCount(out var idsCount) || idsCount == 0)
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(ids));

        var filter = Builders<TDocument>.Filter.In("_id", ids.ToArray());

        var results = await FindAsync(filter, cancellationToken);

        return results ?? Enumerable.Empty<TDocument>();
    }
}