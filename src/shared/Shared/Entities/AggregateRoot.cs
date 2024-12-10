using Habanerio.Xpnss.Shared.Events;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Shared.Entities;

public abstract class AggregateRoot<TId>(TId id) :
    Entity<TId>(id)
    where TId : EntityId
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> IntegrationEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}