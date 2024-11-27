using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;

/// <summary>
/// Lets other parts of the system know that a transaction has been created.
/// </summary>
public record TransactionCreatedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; }

    public string AccountId { get; }

    public string CategoryId { get; }

    public string PayerPayeeId { get; }

    public string TransactionId { get; }

    public TransactionTypes.Keys TransactionType { get; }

    public decimal Amount { get; }

    public DateTime DateOfTransaction { get; set; }

    public TransactionCreatedIntegrationEvent(
        string transactionId,
        string userId,
        string accountId,
        string categoryId,
        string payerPayeeId,
        TransactionTypes.Keys transactionType,
        decimal amount,
        DateTime dateOfTransaction)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userId));

        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(accountId));

        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(transactionId));

        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Value must be greater than zero.");

        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        PayerPayeeId = payerPayeeId;
        TransactionId = transactionId;
        TransactionType = transactionType;
        Amount = amount;
        DateOfTransaction = dateOfTransaction;
    }
}