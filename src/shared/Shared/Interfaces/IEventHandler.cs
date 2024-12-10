namespace Habanerio.Xpnss.Shared.Interfaces;

public interface IEventHandler<in TEvent> where TEvent : IIntegrationEvent
{
    Task Handle(TEvent @event);
}