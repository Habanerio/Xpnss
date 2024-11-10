using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain;

public abstract class Entity(string id)
{
    public string Id { get; protected set; } = id;

    public bool IsTransient { get; init; }
}

public abstract class Entity<TId>(TId id)
    where TId : EntityId
{
    public TId Id { get; protected set; } = id;

    public bool IsTransient { get; init; }
}