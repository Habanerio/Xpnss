using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;

/// <summary>
/// Lets other parts of the system know that a transaction has been deleted.
/// </summary>
public sealed record TransactionDeletedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; }

    public string AccountId { get; }

    public string MerchantId { get; }

    public string TransactionId { get; }

    public TransactionEnums.TransactionKeys TransactionType { get; }

    public decimal Amount { get; }

    public DateTime DateOfTransaction { get; set; }

    //public DateTime DateOfTransactionUtc => DateOfTransaction.ToUniversalTime();


    public TransactionDeletedIntegrationEvent(
        string transactionId,
        string userId,
        string accountId,
        string payerPayeeId,
        TransactionEnums.TransactionKeys transactionType,
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
        MerchantId = payerPayeeId;
        TransactionId = transactionId;
        TransactionType = transactionType;
        Amount = amount;
        DateOfTransaction = dateOfTransaction;
    }
}