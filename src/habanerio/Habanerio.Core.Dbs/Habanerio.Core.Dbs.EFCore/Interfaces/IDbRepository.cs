using System.Linq.Expressions;

namespace Habanerio.Core.DBs.EFCore.Interfaces;

public interface IDbRepository<TDbEntity, in TId> where TDbEntity : IDbEntity<TId> where TId : IComparable<TId>, IEquatable<TId>
{
    bool Exists(TId id);

    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a specific entity based on its id of type TId.
    /// </summary>
    /// <param name="id">The unique Id for the entity.</param>
    /// <returns>An entity of type T</returns>
    TDbEntity? Find(TId id);

    /// <summary>
    /// Gets a collection of one or more backlinks by their id.
    /// </summary>
    /// <param name="ids">The ids.</param>
    /// <returns></returns>
    IEnumerable<TDbEntity> Find(IEnumerable<TId> ids);

    /// <summary>
    /// Finds a specific entity based on its id of type TId.
    /// </summary>
    /// <param name="id">The unique Id for the entity.</param>
    /// <param name="cancellationToken">The cancellation cancellationToken to end the request</param>
    /// <returns>An entity of type T</returns>
    ValueTask<TDbEntity?> FindAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a collection of one or more T by their id.
    /// </summary>
    /// <param name="ids">The ids.</param>
    /// <param name="cancellationToken">The cancellation cancellationToken to end the request</param>
    /// <returns></returns>
    ValueTask<IEnumerable<TDbEntity>> FindAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    //Task<(IEnumerable<TDbEntity> Results, int TotalPages, int TotalCount)> GetAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);

    TDbEntity? FirstOrDefault(Expression<Func<TDbEntity, bool>> expression, CancellationToken cancellationToken = default);

    Task<TDbEntity?> FirstOrDefaultAsync(Expression<Func<TDbEntity, bool>> expression, CancellationToken cancellationToken = default);


    #region - CRUD -
    /// <summary>
    /// Adds a single entity of type T to the repository.
    /// </summary>
    /// <param name="entity">The entity to be saved.</param>
    void Add(TDbEntity entity);

    /// <summary>
    /// Adds a collection of entities of type T to the repository.
    /// </summary>
    /// <param name="entities">The entity.</param>
    void AddRange(IEnumerable<TDbEntity> entities);

    Task AddRangeAsync(IEnumerable<TDbEntity> entities, CancellationToken cancellationToken = default);

    void Remove(TId id);

    /// <summary>
    /// Removes a single entity of type T from the repository.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Remove(TDbEntity entity);

    Task RemoveAsync(TId id, CancellationToken cancellationToken = default);

    Task RemoveAsync(TDbEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a collection of entities.
    /// </summary>
    /// <param name="ids">The ids of the entities to be removed.</param>
    void RemoveRange(IEnumerable<TId> ids);

    Task RemoveRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    void Update(TDbEntity entity);

    //Task<long> UpdateAsync(TDbEntity entity, CancellationToken cancellationToken = default);

    #endregion


}