using System.Linq.Expressions;

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
    Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> FindDocumentsAsync(
    Expression<Func<TDocument, bool>> predicate,
    int pageNo,
    int pageSize,
    CancellationToken cancellationToken = default);

    Task<IEnumerable<TDocument>> FindDocumentsAsync(
        Expression<Func<TDocument, bool>> predicate,
        CancellationToken cancellationToken = default);


    Task<TDocument?> FirstOrDefaultDocumentAsync(
        Expression<Func<TDocument, bool>> predicate,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Gets a single document by its Id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TDocument> GetDocumentAsync(
        TId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple documents by their Ids.
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<TDocument>> GetDocumentAsync(
        IEnumerable<TId> ids,
        CancellationToken cancellationToken = default);


    Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> ListDocumentsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all documents in the collection.
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<(IEnumerable<TDocument> Results, int TotalPages, int TotalCount)> ListDocumentsAsync(
        int pageNo,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a single entity of type T from the repository.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    bool RemoveDocument(TDocument entity, CancellationToken cancellationToken = default);

    Task<bool> RemoveDocumentAsync(TId id, CancellationToken cancellationToken = default);

    Task<bool> RemoveDocumentAsync(TDocument entity, CancellationToken cancellationToken = default);

    //Task<bool> RemoveDocumentAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a collection of entities.
    /// </summary>
    /// <param name="ids">The ids of the entities to be removed.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    bool RemoveDocumentRange(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    Task<bool> RemoveDocumentRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a document in the collection.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> UpdateDocumentAsync(TDocument document, CancellationToken cancellationToken = default);
}