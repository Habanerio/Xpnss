using MediatR;

namespace Habanerio.Xpnss.Shared.Interfaces;

public interface IDomainEvent : IRequest
{
    Guid Id { get; }

    DateTime CreationDate { get; }
}