using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

/// <summary>
/// Represents a withdrawal of money from an account.
/// </summary>
public class WithdrawalTransaction : CreditTransaction
{
    private WithdrawalTransaction(
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "") :
        base(
            userId,
            accountId,
            TransactionEnums.TransactionKeys.WITHDRAWAL,
            payerPayeeId,
            amount,
            description,
            transactionDate,
            tags,
            extTransactionId)
    { }

    private WithdrawalTransaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags,
        string extTransactionId,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            accountId,
            TransactionEnums.TransactionKeys.WITHDRAWAL,
            payerPayeeId,
            amount,
            description,
            transactionDate,
            tags,
            extTransactionId,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }

    public static WithdrawalTransaction Load(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags,
        string extTransactionId,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) =>
        new WithdrawalTransaction(
            id,
            userId,
            accountId,
            amount,
            description,
            payerPayeeId,
            transactionDate,
            tags,
            extTransactionId,
            dateCreated,
            dateUpdated,
            dateDeleted);

    public static WithdrawalTransaction New(
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "") =>
        new WithdrawalTransaction(
            userId,
            accountId,
            amount,
            description,
            payerPayeeId,
            transactionDate,
            tags,
            extTransactionId);
}
