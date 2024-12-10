using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;

/// <summary>
/// Lets other parts of the system know that a transaction has been created.
/// Used in:
/// - Habanerio.Xpnss.MonthlyTotals.Infrastructure.IntegrationEvents.EventHandlers.TransactionCreatedIntegrationEventHandler
/// - Habanerio.Xpnss.Accounts.Infrastructure.IntegrationEvents.EventHandlers.TransactionCreatedIntegrationEventHandler
/// </summary>
public sealed record TransactionCreatedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; }

    public string AccountId { get; }

    public string CategoryId { get; }

    public string SubCategoryId { get; set; }

    public string PayerPayeeId { get; }

    public string TransactionId { get; }

    public TransactionEnums.TransactionKeys TransactionType { get; }

    public decimal Amount { get; }

    public DateTime DateOfTransaction { get; set; }

    public TransactionCreatedIntegrationEvent(
        string transactionId,
        string userId,
        string accountId,
        string categoryId,
        string subCategoryId,
        string payerPayeeId,
        TransactionEnums.TransactionKeys transactionType,
        decimal amount,
        DateTime dateOfTransaction)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userId));

        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(transactionId));

        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Value must be greater than zero.");

        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        SubCategoryId = subCategoryId;
        PayerPayeeId = payerPayeeId;
        TransactionId = transactionId;
        TransactionType = transactionType;
        Amount = amount;
        DateOfTransaction = dateOfTransaction;
    }
}