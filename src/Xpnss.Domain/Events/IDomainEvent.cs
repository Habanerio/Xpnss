using MediatR;

namespace Habanerio.Xpnss.Domain.Events;

public interface IDomainEvent : IRequest
{
    Guid Id { get; }

    DateTime CreationDate { get; }
}