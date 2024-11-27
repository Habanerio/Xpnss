using MediatR;

namespace Habanerio.Xpnss.Infrastructure.IntegrationEvents;

public interface IIntegrationEventHandler<in TIntegrationEvent> : INotificationHandler<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{ }