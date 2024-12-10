namespace Habanerio.Xpnss.Shared.Interfaces;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event)
        where TEvent : IIntegrationEvent;
}