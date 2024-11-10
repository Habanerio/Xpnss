namespace Habanerio.Xpnss.Domain.Events;

public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent @event);
}