using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Shared.IntegrationEvents.Transactions;

/// <summary>
/// Lets other parts of the system know that a transaction has been updated.
/// </summary>
public sealed record TransactionUpdatedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; }

    public string AccountId { get; }

    public string MerchantId { get; }

    public string TransactionId { get; }

    public TransactionEnums.TransactionKeys TransactionType { get; }

    public decimal NewAmount { get; }

    public decimal OldAmount { get; }

    public DateTime DateOfTransaction { get; set; }

    public decimal Difference => NewAmount - OldAmount;

    public TransactionUpdatedIntegrationEvent(
        string transactionId,
        string userId,
        string accountId,
        string payerPayeeId,
        TransactionEnums.TransactionKeys transactionType,
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
        MerchantId = payerPayeeId;
        TransactionId = transactionId;
        TransactionType = transactionType;
        NewAmount = newAmount;
        OldAmount = oldAMount;
        DateOfTransaction = dateOfTransaction;
    }
}