using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public class CreditTransaction : TransactionBase
{
    /// <summary>
    /// New with one Item
    /// </summary>
    protected CreditTransaction(
            UserId userId,
            AccountId accountId,
            string description,
            string extTransactionId,
            TransactionItem item,
            PayerPayeeId payerPayeeId,
            IEnumerable<string>? tags,
            DateTime transactionDate,
            TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            isCredit: true,
            item,
            payerPayeeId,
            tags,
            transactionDate,
            transactionType)
    { }

    ///// <summary>
    ///// New with multiple Items
    ///// </summary>
    //protected CreditTransaction(
    //    UserId userId,
    //    AccountId accountId,
    //    string description,
    //    string extTransactionId,
    //    IEnumerable<TransactionItem> items,
    //    PayerPayeeId payerPayeeId,
    //    IEnumerable<string>? tags,
    //    DateTime transactionDate,
    //    TransactionEnums.TransactionKeys transactionType) :
    //    base(
    //        userId,
    //        accountId,
    //        description,
    //        extTransactionId,
    //        isCredit: true,
    //        items,
    //        payerPayeeId,
    //        tags,
    //        transactionDate,
    //        transactionType)
    //{ }

    /// <summary>
    /// Existing with a single Item
    /// </summary>
    protected CreditTransaction(
            TransactionId id,
            UserId userId,
            AccountId accountId,
            string description,
            string extTransactionId,
            TransactionItem item,
            PayerPayeeId payerPayeeId,
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
            isCredit: true,
            item,
            payerPayeeId,
            tags,
            transactionDate,
            transactionType,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }

    public static CreditTransaction NewDeposit(
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "")
    {
        return new CreditTransaction(
            userId,
            accountId,
            description,
            extTransactionId,
            TransactionItem.New(new Money(amount), CategoryId.Empty, SubCategoryId.Empty, description),
            payerPayeeId,
            tags,
            transactionDate,
            TransactionEnums.TransactionKeys.DEPOSIT);
    }

    public static CreditTransaction NewPayment(
        UserId userId,
        AccountId accountId,
        string description,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "")
    {
        return new CreditTransaction(
            userId,
            accountId,
            description,
            extTransactionId,
            item,
            payerPayeeId,
            tags,
            transactionDate,
            TransactionEnums.TransactionKeys.PAYMENT_OUT);
    }

    public static CreditTransaction Load(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionId,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new CreditTransaction(
            id,
            userId,
            accountId,
            description,
            extTransactionId,
            item,
            payerPayeeId,
            tags,
            transactionDate,
            transactionType,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

}