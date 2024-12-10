using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public class DepositTransaction :
    CreditTransaction
{
    protected DepositTransaction(
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
            TransactionEnums.TransactionKeys.DEPOSIT,
            payerPayeeId,
            amount,
            description,
            transactionDate,
            tags,
            extTransactionId)
    { }

    protected DepositTransaction(
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
            TransactionEnums.TransactionKeys.DEPOSIT,
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

    public static DepositTransaction Load(
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
        DateTime? dateDeleted = null)
    {
        return new DepositTransaction(
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
    }

    public static DepositTransaction New(
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "")
    {
        return new DepositTransaction(
            userId,
            accountId,
            amount,
            description,
            payerPayeeId,
            transactionDate,
            tags,
            extTransactionId);
    }
}