using Habanerio.Xpnss.Shared.Interfaces;

namespace Habanerio.Xpnss.Shared.IntegrationEvents;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime CreationDate { get; } = DateTime.UtcNow;
}