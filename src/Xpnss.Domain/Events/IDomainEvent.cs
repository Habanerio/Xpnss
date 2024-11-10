using MediatR;

namespace Habanerio.Xpnss.Domain.Events;

public interface IDomainEvent : IRequest
{
    Guid Id { get; }

    DateTime OccurredOn { get; }
}

public interface IDomainEvent<out TResult> : IRequest<TResult>
{
    Guid Id { get; }

    DateTime OccurredOn { get; }
}