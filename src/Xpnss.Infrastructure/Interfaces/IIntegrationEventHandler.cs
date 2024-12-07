//using Habanerio.Xpnss.Infrastructure.IntegrationEvents;
//using MediatR;

//namespace Habanerio.Xpnss.Infrastructure.Interfaces;

//public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
//    where TIntegrationEvent : IIntegrationEvent
//{
//    Task Handle(TIntegrationEvent @event);

//    Task IIntegrationEventHandler.Handle(IIntegrationEvent @event) => Handle(@event);
//}

//public interface IIntegrationEventHandler
//{
//    Task Handle(IIntegrationEvent @event);
//}

////public interface IIntegrationEventHandler<in TIntegrationEvent> : 
////    INotificationHandler<TIntegrationEvent>
////    where TIntegrationEvent : IIntegrationEvent
////{ }

using Habanerio.Xpnss.Infrastructure.Interfaces;
using MediatR;

///
public interface IIntegrationEventHandler<in TIntegrationEvent> : INotificationHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{

}