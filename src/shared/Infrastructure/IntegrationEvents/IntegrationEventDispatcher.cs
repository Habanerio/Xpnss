using Habanerio.Xpnss.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Infrastructure.IntegrationEvents;

/// <summary>
/// Responsible for dispatching Domain related Events to their respective handlers.
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="logger"></param>
public class IntegrationEventDispatcher(
    IServiceProvider serviceProvider,
    ILogger<IntegrationEventDispatcher> logger) : IEventDispatcher
{
    private readonly ILogger<IntegrationEventDispatcher> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    private readonly IServiceProvider _serviceProvider = serviceProvider ??
        throw new ArgumentNullException(nameof(serviceProvider));

    public async Task DispatchAsync<TEvent>(TEvent @event)
        where TEvent : IIntegrationEvent
    {
        var t = typeof(TEvent);

        using var scope = _serviceProvider.CreateScope();

        var handler = scope.ServiceProvider.GetService<IEventHandler<TEvent>>();

        if (handler == null)
        {
            _logger.LogWarning("No handler registered for event type {EventType}", typeof(TEvent).Name);
            return;
        }

        try
        {
            await handler.Handle(@event);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while handling event {EventType}", typeof(TEvent).Name);
        }

    }
}