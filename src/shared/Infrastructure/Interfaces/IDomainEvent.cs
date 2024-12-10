using MediatR;

namespace Habanerio.Xpnss.Infrastructure.Interfaces;

public interface IDomainEvent : IRequest
{
    Guid Id { get; }

    DateTime CreationDate { get; }
}