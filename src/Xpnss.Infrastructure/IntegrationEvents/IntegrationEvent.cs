using Habanerio.Xpnss.Infrastructure.Interfaces;

namespace Habanerio.Xpnss.Infrastructure.IntegrationEvents;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime CreationDate { get; } = DateTime.UtcNow;
}