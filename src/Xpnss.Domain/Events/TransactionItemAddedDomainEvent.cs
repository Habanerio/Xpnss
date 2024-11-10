using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Events;

public class TransactionItemAddedDomainEvent(
    TransactionItemId Id,
    Money Amount,
    CategoryId CategoryId,
    string Description) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}