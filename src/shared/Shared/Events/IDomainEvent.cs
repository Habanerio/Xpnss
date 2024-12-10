using MediatR;

namespace Habanerio.Xpnss.Shared.Events;

public interface IDomainEvent : IRequest
{
    Guid Id { get; }

    DateTime CreationDate { get; }
}