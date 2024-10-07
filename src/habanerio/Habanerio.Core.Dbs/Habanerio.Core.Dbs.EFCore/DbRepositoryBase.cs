using System.Linq.Expressions;
using Habanerio.Core.DBs.EFCore.Expressions;
using Habanerio.Core.DBs.EFCore.Extensions;
using Habanerio.Core.DBs.EFCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habanerio.Core.DBs.EFCore;

public abstract class DbRepositoryBase<TDbEntity, TId> : IDbRepository<TDbEntity, TId> where TDbEntity : class, IDbEntity<TId> where TId : IComparable<TId>, IEquatable<TId>
{
    private const string EXCEPTION_COLLECTION_NOT_FOUND = "The collection was not found";

    private const string EXCEPTION_ENTITY_NOT_FOUND = "The entity with the id '{0}' was not found";
    private const string EXCEPTION_ID_CANT_BE_EMPTY = "The Id cannot be null or empty";

    private const string EXCEPTION_COLLECTION_CANT_BE_NULL_OR_EMPTY = "The collection cannot be null or empty";
    private const string EXCEPTION_IDS_CANT_BE_EMPTY = "The Ids collection cannot be null or empty";

    protected DbContextBase Context { get; set; }

    protected abstract DbSet<TDbEntity> DbSet { get; }

    protected DbRepositoryBase(DbContextBase context)
    {
        Context = context;
        //DbSet = Context.Set<TDbEntity>();
    }

    public virtual bool Exists(TId id)
    {
        if (string.IsNullOrWhiteSpace(id?.ToString()))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        // TId.Id needs an Equal override
        return Find(id) != null;
    }

    public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id?.ToString()))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        return (await FindAsync(id, cancellationToken)) != null;
    }

    public virtual TDbEntity? Find(TId id)
    {
        if (string.IsNullOrWhiteSpace(id?.ToString()))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        return DbSet.Find(id);
    }

    public virtual IEnumerable<TDbEntity> Find(IEnumerable<TId> ids)
    {
        var idsArray = ids?.ToArray() ?? Array.Empty<TId>();

        if (!idsArray.Any())
            throw new ArgumentException(EXCEPTION_IDS_CANT_BE_EMPTY, nameof(ids));

        var results = DbSet.Where(x => idsArray.Contains(x.Id)).AsEnumerable();

        return results;
    }

    public virtual ValueTask<TDbEntity?> FindAsync(TId id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id?.ToString()))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        return DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async ValueTask<IEnumerable<TDbEntity>> FindAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        var idsArray = ids?.ToArray() ?? Array.Empty<TId>();

        if (!idsArray.Any())
            throw new ArgumentException(EXCEPTION_IDS_CANT_BE_EMPTY, nameof(ids));

        return await DbSet.Where(x => idsArray.Contains(x.Id)).ToArrayAsync(cancellationToken);
    }

    public async Task<IEnumerable<TDbEntity>> FindAsync(Expression<Func<TDbEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        var qry = DbSet.AsQueryable();

        TDbEntity[] results = Array.Empty<TDbEntity>();

        try
        {
            results = await qry.Where(filter).ToArrayAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return results;
    }


    public virtual TDbEntity? FirstOrDefault(Expression<Func<TDbEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefault(expression);
    }

    public virtual Task<TDbEntity?> FirstOrDefaultAsync(Expression<Func<TDbEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(expression, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// This should only be used by other repositories that need to search for a specific entities.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="orderBys"></param>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentOutOfRangeException">If pageNo is less than 1</exception>
    /// <exception cref="ArgumentOutOfRangeException">If pageSize is less than 1</exception>
    /// <returns></returns>
    protected async Task<PagedResults<TDbEntity>> FilterAsync(
        Expression<Func<TDbEntity, bool>> filter,
        int pageNo,
        int pageSize,
        OrderByExpression<TDbEntity>? orderBys = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNo < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNo), "The page number must be greater than 0");

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "The page size must be greater than 0");

        var qry = DbSet.AsQueryable();

        //if (filter != null)
        qry = qry.Where(filter);

        //var orderByArr = orderBys?.GetOrderBys() ??
        //                 OrderByExpression<TDbEntity>.Empty();

        //if (orderByArr.Any())
        //{
        //    foreach (var orderBy in orderByArr)
        //    {
        //        if (orderBy.IsDescending)
        //            qry = qry.OrderByDescending(orderBy.KeySelector);
        //        else
        //            qry = qry.OrderBy(orderBy.KeySelector);
        //    }
        //}

        qry = qry.ApplyOrderBy(orderBys);

        var totalCount = await qry.CountAsync(cancellationToken);

        var results = qry
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize).AsNoTracking()
            .AsEnumerable();

        return new PagedResults<TDbEntity>(results, pageNo, pageSize, totalCount);
    }

    protected async Task<IEnumerable<TDbEntity>> FilterAsync(
        Expression<Func<TDbEntity, bool>> filter,
        OrderByExpression<TDbEntity>? orderBys = null,
        CancellationToken cancellationToken = default)
    {
        var qry = DbSet.AsQueryable();

        //if (filter != null)
        qry = qry.Where(filter);

        //var orderByArr = orderBys?.GetOrderBys() ??
        //                 OrderByExpression<TDbEntity>.Empty();

        //if (orderByArr.Any())
        //{
        //    foreach (var orderBy in orderByArr)
        //    {
        //        if (orderBy.IsDescending)
        //            qry = qry.OrderByDescending(orderBy.KeySelector);
        //        else
        //            qry = qry.OrderBy(orderBy.KeySelector);
        //    }
        //}

        qry = qry.ApplyOrderBy(orderBys);

        var totalCount = await qry.CountAsync(cancellationToken);

        var results = qry
            .AsEnumerable();

        return results;
    }

    /// <summary>
    /// Adds an entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to be added</param>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual void Add(TDbEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        DbSet.Add(entity);
    }


    public virtual void AddRange(IEnumerable<TDbEntity> entities)
    {
        if (entities is null)
            throw new ArgumentNullException(nameof(entities), EXCEPTION_COLLECTION_CANT_BE_NULL_OR_EMPTY);

        var entitiesArray = entities.ToArray();

        if (!entitiesArray.Any())
            throw new ArgumentException(EXCEPTION_COLLECTION_CANT_BE_NULL_OR_EMPTY, nameof(entities));

        DbSet.AddRange(entitiesArray);
    }

    public virtual Task AddRangeAsync(IEnumerable<TDbEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities is null)
            throw new ArgumentNullException(nameof(entities), EXCEPTION_COLLECTION_CANT_BE_NULL_OR_EMPTY);

        var entitiesArray = entities.ToArray();

        if (!entitiesArray.Any())
            throw new ArgumentException(EXCEPTION_COLLECTION_CANT_BE_NULL_OR_EMPTY, nameof(entities));

        return DbSet.AddRangeAsync(entitiesArray, cancellationToken);
    }


    /// <summary>
    /// This flags the entity with this id, to be deleted from the repository.
    /// It will not be deleted until Save is called.
    /// </summary>
    /// <param name="id">The id of the TDbEntity to be deleted</param>
    /// <exception cref="ArgumentException">If no TDbEntity can be found with this id</exception>
    public virtual void Remove(TId id)
    {
        RemoveRange([id]);
    }

    /// <summary>
    /// This marks the entity to be deleted from the repository.
    /// It will not be deleted until Save is called.
    /// </summary>
    /// <param name="entity">The entity to be flagged as deleted</param>
    /// <exception cref="ArgumentNullException">If the entity is null</exception>
    public virtual void Remove(TDbEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        DbSet.Remove(entity);
    }

    /// <summary>
    /// Roll your own implementation of this method.
    /// There is no native implementation for this method.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task RemoveAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flags a collection of TDbEntity to be deleted from the repository.
    /// They will not be deleted until Save is called.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task RemoveAsync(TDbEntity entity, CancellationToken cancellationToken = default);


    /// <summary>
    /// Flags a collection of TDbEntity to be deleted from the repository.
    /// They will not be deleted until Save is called.
    /// </summary>
    /// <param name="ids"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public virtual void RemoveRange(IEnumerable<TId> ids)
    {
        if (ids is null)
            throw new ArgumentNullException(nameof(ids), EXCEPTION_IDS_CANT_BE_EMPTY);

        var idsArray = ids.ToArray();

        if (!idsArray.Any())
            throw new ArgumentException(EXCEPTION_IDS_CANT_BE_EMPTY, nameof(ids));

        var entities = Find(idsArray);

        DbSet.RemoveRange(entities);
    }

    /// <summary>
    /// Roll your own implementation of this method.
    /// There is no native implementation for this method.
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task RemoveRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Roll your own implementation of this method.
    /// There is no native implementation for this method.
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task RemoveRangeAsync(IEnumerable<TDbEntity> entities, CancellationToken cancellationToken = default);

    public virtual int SaveChanges()
    {
        var count = Context.SaveChanges();

        return count;
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var count = await Context.SaveChangesAsync(cancellationToken);

        return count;
    }

    public virtual void Update(TDbEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        DbSet.Update(entity);
    }
}