namespace Habanerio.Xpnss.Domain.Events;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event)
        where TEvent : IDomainEvent;
}