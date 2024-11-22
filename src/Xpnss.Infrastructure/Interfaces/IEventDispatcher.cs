namespace Habanerio.Xpnss.Infrastructure.Interfaces;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event)
        where TEvent : IIntegrationEvent;
}