namespace Habanerio.Xpnss.Infrastructure.Interfaces;

public interface IEventHandler<in TEvent> where TEvent : IIntegrationEvent
{
    Task Handle(TEvent @event);
}