using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public class DebitTransaction : Transaction
{
    /// <summary>
    /// New transaction with one Item
    /// </summary>
    protected DebitTransaction(
            UserId userId,
            AccountId accountId,
            string description,
            string extTransactionId,
            TransactionItem item,
            PayerPayeeId payerPayeeId,
            RefTransactionId refTransactionId,
            IEnumerable<string>? tags,
            DateTime transactionDate,
            TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            isCredit: false,
            item,
            payerPayeeId,
            refTransactionId,
            tags,
            transactionDate,
            transactionType)
    { }

    /// <summary>
    /// New transaction with multiple Items
    /// </summary>
    protected DebitTransaction(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionId,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            isCredit: false,
            items,
            payerPayeeId,
            refTransactionId,
            tags,
            transactionDate,
            transactionType)
    { }

    /// <summary>
    /// Existing transaction with one Item
    /// </summary>
    protected DebitTransaction(
            TransactionId id,
            UserId userId,
            AccountId accountId,
            string description,
            string extTransactionId,
            TransactionItem item,
            PayerPayeeId payerPayeeId,
            RefTransactionId refTransactionId,
            IEnumerable<string>? tags,
            DateTime transactionDate,
            TransactionEnums.TransactionKeys transactionType,
            DateTime dateCreated,
            DateTime? dateUpdated = null,
            DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            accountId,
            description,
            extTransactionId,
            isCredit: false,
            item,
            payerPayeeId,
            refTransactionId,
            tags,
            transactionDate,
            transactionType,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }

    /// <summary>
    /// Existing transaction multiple Items
    /// </summary>
    protected DebitTransaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionId,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            accountId,
            description,
            extTransactionId,
            isCredit: false,
            items,
            payerPayeeId,
            refTransactionId,
            tags,
            transactionDate,
            transactionType,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }

    public static DebitTransaction NewWithdrawal(
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        RefTransactionId refTransactionId,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "")
    {
        return new DebitTransaction(
            userId,
            accountId,
            description,
            extTransactionId,
            TransactionItem.New(amount, CategoryId.Empty, SubCategoryId.Empty, description),
            payerPayeeId,
            refTransactionId,
            tags,
            transactionDate,
            TransactionEnums.TransactionKeys.WITHDRAWAL);
    }

    // Payment may be its own transaction type in the future
    // As a payment can go from one Account to another
    public static DebitTransaction NewPayment(
        UserId userId,
        AccountId accountId,
        string description,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        RefTransactionId refTransactionId,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "")
    {
        return new DebitTransaction(
            userId,
            accountId,
            description,
            extTransactionId,
            item,
            payerPayeeId,
            refTransactionId,
            tags,
            transactionDate,
            TransactionEnums.TransactionKeys.PAYMENT_OUT);
    }

    public static DebitTransaction Load(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionId,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new DebitTransaction(
            id,
            userId,
            accountId,
            description,
            extTransactionId,
            item,
            payerPayeeId,
            refTransactionId,
            tags,
            transactionDate,
            transactionType,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }
}