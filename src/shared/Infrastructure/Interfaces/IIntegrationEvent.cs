using System.Text.Json.Serialization;
using MediatR;

namespace Habanerio.Xpnss.Infrastructure.Interfaces;

public interface IIntegrationEvent : INotification
{
    [JsonInclude]
    Guid Id { get; }

    [JsonInclude]
    DateTime CreationDate { get; }
}