using MediatR;

namespace Habanerio.Xpnss.Shared.IntegrationEvents;

public interface IIntegrationEventHandler<in TIntegrationEvent> : INotificationHandler<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{ }