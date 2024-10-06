using System.Linq.Expressions;
using MongoDB.Bson;

namespace Habanerio.Core.Dbs.MongoDb.Interfaces;

public interface IMongoDbRepository<TDocument, in TId> where TDocument : IMongoDocument
{
    void AddDocument(TDocument document);

    Task AddDocumentAsync(TDocument document, CancellationToken cancellationToken = default);

    void AddDocuments(IEnumerable<TDocument> documents);

    Task AddDocumentsAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default);


    /// <summary>
    /// Attempts to find one or more documents that match the given predicate.
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="pageNo">The number of the page of results to return</param>
    /// <param name="pageSize">The size of the page of results to return</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> FindAsync(
    Expression<Func<TDocument, bool>> predicate,
    int pageNo,
    int pageSize,
    CancellationToken cancellationToken = default);

    Task<IEnumerable<TDocument>> FindAsync(
        Expression<Func<TDocument, bool>> predicate,
        CancellationToken cancellationToken = default);


    Task<TDocument?> FirstOrDefaultAsync(
        Expression<Func<TDocument, bool>> predicate,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Gets a single document by its Id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TDocument> GetAsync(
        TId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple documents by their Ids.
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<TDocument>> GetAsync(
        IEnumerable<TId> ids,
        CancellationToken cancellationToken = default);


    Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> ListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all documents in the collection.
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> ListAsync(
        int pageNo,
        int pageSize,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Updates a document in the collection.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> UpdateDocumentAsync(TDocument document, CancellationToken cancellationToken = default);
}

public interface IMongoDbRepository<TDocument> : IMongoDbRepository<TDocument, ObjectId> where TDocument : IMongoDocument;