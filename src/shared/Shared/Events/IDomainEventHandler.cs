namespace Habanerio.Xpnss.Shared.Events;

public interface IDomainEventHandler<in TIntegrationEvent> : IDomainEventHandler
    where TIntegrationEvent : IDomainEvent
{
    Task Handle(TIntegrationEvent @event);

    Task IDomainEventHandler.Handle(IDomainEvent @event) => Handle((TIntegrationEvent)@event);
}

public interface IDomainEventHandler
{
    Task Handle(IDomainEvent @event);
}