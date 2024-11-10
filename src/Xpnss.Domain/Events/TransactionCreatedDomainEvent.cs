namespace Habanerio.Xpnss.Domain.Events;

/// <summary>
/// Lets other parts of the system know that a transaction has been created.
/// </summary>
/// <param name="UserId"></param>
/// <param name="AccountId"></param>
/// <param name="TransactionType"></param>
/// <param name="Amount"></param>
/// <param name="DateOfTransaction"></param>
public record TransactionCreatedDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public string UserId { get; set; }

    public string AccountId { get; set; }

    public string TransactionType { get; set; }

    public decimal Amount { get; set; }

    public DateTime DateOfTransactionUtc { get; set; }
}