using FluentResults;
using Habanerio.Core.DBs.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Core.DBs.MongoDB.EFCore;

public class MongoDbRepository<TDbEntity> : DbRepositoryBase<TDbEntity, ObjectId> where TDbEntity : MongoDocument
{
    private const string EXCEPTION_ID_CANT_BE_EMPTY = "The Id cannot be null or empty";
    private const string EXCEPTION_IDS_CANT_BE_EMPTY = "The Ids collection cannot be null or empty";
    private const string EXCEPTION_IDS_CONTAINS_EMPTY_ID = "The Ids collection contains an empty id";

    protected override DbSet<TDbEntity> DbSet => Context.Set<TDbEntity>();

    protected MongoDbRepository(IOptions<MongoDbSettings> options) : base(new MongoDbContext(options))
    {
        //Context = new MongoDbContext<TDbEntity>(options);
    }


    protected MongoDbRepository(MongoDbContext context) : base(context) { }

    public override bool Exists(ObjectId id)
    {
        if (id.Equals(ObjectId.Empty))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        return base.Exists(id);
    }

    public override Task<bool> ExistsAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        if (id.Equals(ObjectId.Empty))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        return base.ExistsAsync(id, cancellationToken);
    }


    public override TDbEntity? Find(ObjectId id)
    {
        if (id.Equals(ObjectId.Empty))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        return base.Find(id);
    }

    public override IEnumerable<TDbEntity> Find(IEnumerable<ObjectId> ids)
    {
        var idsArray = ids?.ToArray() ?? [];

        if (!idsArray.Any())
            throw new ArgumentException(EXCEPTION_IDS_CANT_BE_EMPTY, nameof(ids));

        if (idsArray.Any(i => i.Equals(ObjectId.Empty)))
            throw new ArgumentException(EXCEPTION_IDS_CONTAINS_EMPTY_ID, nameof(ids));

        return base.Find(idsArray);
    }

    public override ValueTask<TDbEntity?> FindAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        if (id.Equals(ObjectId.Empty))
            throw new ArgumentException(EXCEPTION_ID_CANT_BE_EMPTY, nameof(id));

        return base.FindAsync(id, cancellationToken);
    }

    public override ValueTask<IEnumerable<TDbEntity>> FindAsync(IEnumerable<ObjectId> ids, CancellationToken cancellationToken = default)
    {
        var idsArray = ids?.ToArray() ?? [];

        if (!idsArray.Any())
            throw new ArgumentException(EXCEPTION_IDS_CANT_BE_EMPTY, nameof(ids));

        if (idsArray.Any(i => i.Equals(ObjectId.Empty)))
            throw new ArgumentException(EXCEPTION_IDS_CONTAINS_EMPTY_ID, nameof(ids));

        return base.FindAsync(idsArray, cancellationToken);
    }

    /*
    public void Add(TDbEntity entity)
    {
        base.Add(entity);

        Context.ChangeTracker.DetectChanges();
    }

    public override void AddRange(IEnumerable<TDbEntity> entities)
    {
        base.AddRange(entities);

        Context.ChangeTracker.DetectChanges();
    }

    public override Task AddRangeAsync(IEnumerable<TDbEntity> entities, CancellationToken cancellationToken = default)
    {
        base.AddRangeAsync(entities, cancellationToken);

        Context.ChangeTracker.DetectChanges();

        return Task.CompletedTask;
    }

    public override void Remove(ObjectId id)
    {
        base.Remove(id);

        Context.ChangeTracker.DetectChanges();
    }

    public override void Remove(TDbEntity entity)
    {
        base.Remove(entity);

        Context.ChangeTracker.DetectChanges();
    }

    public override Task RemoveAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        base.RemoveAsync(id, cancellationToken);

        Context.ChangeTracker.DetectChanges();

        return Task.CompletedTask;
    }

    public override Task RemoveAsync(TDbEntity entity, CancellationToken cancellationToken = default)
    {
        base.RemoveAsync(entity, cancellationToken);

        Context.ChangeTracker.DetectChanges();

        return Task.CompletedTask;
    }

    public override void RemoveRange(IEnumerable<ObjectId> ids)
    {
        base.RemoveRange(ids);

        Context.ChangeTracker.DetectChanges();
    }
    */

    public override Task RemoveAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveAsync(TDbEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task RemoveRangeAsync(IEnumerable<ObjectId> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();

        if (ids is null)
            throw new ArgumentNullException(nameof(ids), EXCEPTION_IDS_CANT_BE_EMPTY);

        var idsArray = ids.ToArray();

        if (!idsArray.Any())
            throw new ArgumentException(EXCEPTION_IDS_CANT_BE_EMPTY, nameof(ids));

        var entities = await FindAsync(idsArray, cancellationToken);

        DbSet.RemoveRange(entities);
    }

    public override Task RemoveRangeAsync(IEnumerable<TDbEntity> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override void Add(TDbEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        base.Add(entity);
    }
    /*

    public override void Update(TDbEntity entity, CancellationToken cancellationToken = default)
    {
        base.Update(entity, cancellationToken);

        Context.ChangeTracker.DetectChanges();
    }
    */

    public virtual async Task<Result> SaveAsync(CancellationToken cancellationToken = default)
    {
        var numChanges = await base.SaveChangesAsync(cancellationToken);

        return numChanges > 0
            ? Result.Ok()
            : Result.Fail(new Error("No changes were saved"));
    }
}