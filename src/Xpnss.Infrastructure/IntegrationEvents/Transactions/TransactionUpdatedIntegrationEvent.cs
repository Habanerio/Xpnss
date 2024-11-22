using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;

/// <summary>
/// Lets other parts of the system know that a transaction has been updated.
/// </summary>
public record TransactionUpdatedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; }

    public string AccountId { get; }

    public string MerchantId { get; }

    public string TransactionId { get; }

    public TransactionTypes.Keys TransactionType { get; }

    public decimal NewAmount { get; }

    public decimal OldAmount { get; }

    public DateTime DateOfTransaction { get; set; }

    //public DateTime DateOfTransactionUtc => DateOfTransaction.ToUniversalTime();

    public decimal Difference => NewAmount - OldAmount;

    public TransactionUpdatedIntegrationEvent(
        string transactionId,
        string userId,
        string accountId,
        string merchantId,
        TransactionTypes.Keys transactionType,
        decimal oldAMount,
        decimal newAmount,
        DateTime dateOfTransaction)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userId));

        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(accountId));

        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(transactionId));

        if (newAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(newAmount), "Value must be greater than zero.");

        if (oldAMount < 0)
            throw new ArgumentOutOfRangeException(nameof(oldAMount), "Value must be greater than zero.");

        UserId = userId;
        AccountId = accountId;
        MerchantId = merchantId;
        TransactionId = transactionId;
        TransactionType = transactionType;
        NewAmount = newAmount;
        OldAmount = oldAMount;
        DateOfTransaction = dateOfTransaction;
    }
}