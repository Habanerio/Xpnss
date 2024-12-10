using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Shared.Entities;

public enum EntityState
{
    NEW,
    ACTIVE,
    UPDATED,
    DELETED
}

public abstract class Entity(string id)
{
    public string Id { get; protected set; } = id;

    public bool IsDeleted => DateDeleted.HasValue;

    public bool IsTransient { get; init; }

    public DateTime DateCreated { get; init; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }

    public virtual EntityState EntityState
    {
        get
        {
            if (IsTransient)
                return EntityState.NEW;

            if (IsDeleted)
                return EntityState.DELETED;

            return EntityState.ACTIVE;
        }
    }
}

public abstract class Entity<TId>(TId id)
    where TId : EntityId
{
    public TId Id { get; protected set; } = id ??
        throw new ArgumentNullException(nameof(id), "EntityId cannot be null");

    public bool IsDeleted => DateDeleted.HasValue;

    public bool IsTransient { get; init; }

    public DateTime DateCreated { get; init; } = DateTime.UtcNow;

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }

    public virtual EntityState EntityState
    {
        get
        {
            if (IsTransient)
                return EntityState.NEW;

            if (IsDeleted)
                return EntityState.DELETED;

            return EntityState.ACTIVE;
        }
    }
}