using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public abstract class PaymentTransaction :
    Transaction
{
    public bool IsExistingAccount { get; set; }

    protected PaymentTransaction(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        bool isCredit,
        bool isExistingAccount,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            description,
            extTransactionNo,
            isCredit,
            item,
            payerPayeeId,
            tags,
            transactionDate,
            transactionType)
    {
        IsExistingAccount = isExistingAccount;
    }

    protected PaymentTransaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        bool isCredit,
        bool isExistingAccount,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) :
        base(
            id,
            userId,
            accountId,
            description,
            extTransactionNo,
            isCredit,
            item,
            payerPayeeId,
            tags,
            transactionDate,
            transactionType,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        IsExistingAccount = isExistingAccount;
    }
}


public sealed class PaymentInTransaction :
    CreditTransaction
{
    public bool IsOwnAccount { get; }

    /// <summary>
    /// New with one Item
    /// </summary>
    private PaymentInTransaction(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        bool isPaidFromOwnAccount,
        TransactionItem item,
        PayerPayeeId paidFromId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            description,
            extTransactionNo,
            item,
            paidFromId,
            //refTransactionId,
            tags,
            transactionDate,
            transactionType: TransactionEnums.TransactionKeys.PAYMENT_IN)
    {
        IsOwnAccount = isPaidFromOwnAccount;
    }

    public PaymentInTransaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        TransactionItem item,
        PayerPayeeId paidFromId,
        bool isPaidFromOwnAccount,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) :
        base(
            id,
            userId,
            accountId,
            description,
            extTransactionNo,
            item,
            paidFromId,
            //refTransactionId,
            tags,
            transactionDate,
            transactionType: TransactionEnums.TransactionKeys.PAYMENT_IN,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        IsOwnAccount = isPaidFromOwnAccount;
    }

    public static PaymentInTransaction New(
        UserId userId,
        AccountId accountId,
        Money amount,
        CategoryId categoryId,
        string description,
        string extTransactionNo,
        bool isPaidFromOwnAccount,
        PayerPayeeId paidFromId,
        //RefTransactionId refTransactionId,
        SubCategoryId subCategoryId,
        IEnumerable<string>? tags,
        DateTime transactionDate)
    {
        return new PaymentInTransaction(
            userId,
            accountId,
            description,
            extTransactionNo,
            isPaidFromOwnAccount,
            TransactionItem.New(
                new Money(amount),
                categoryId,
                subCategoryId,
                description),
            paidFromId,
            tags,
            transactionDate);
    }
}

public sealed class PaymentOutTransaction :
    DebitTransaction
{
    public bool IsPaidToOwnAccount { get; }

    /// <summary>
    /// New with one Item
    /// </summary>
    private PaymentOutTransaction(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        bool isPaidToOwnAccount,
        TransactionItem item,
        PayerPayeeId paidToId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            description,
            extTransactionNo,
            item,
            paidToId,
            //refTransactionId,
            tags,
            transactionDate,
            transactionType: TransactionEnums.TransactionKeys.PAYMENT_OUT)
    {
        IsPaidToOwnAccount = isPaidToOwnAccount;
    }

    /// <summary>
    /// Existing with a single Item
    /// </summary>
    public PaymentOutTransaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        bool isPaidToOwnAccount,
        TransactionItem item,
        PayerPayeeId paidToId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) :
        base(
            id,
            userId,
            accountId,
            description,
            extTransactionNo,
            item,
            paidToId,
            //refTransactionId,
            tags,
            transactionDate,
            transactionType: TransactionEnums.TransactionKeys.PAYMENT_OUT,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        IsPaidToOwnAccount = isPaidToOwnAccount;
    }

    public static PaymentOutTransaction New(
        UserId userId,
        AccountId accountId,
        Money amount,
        CategoryId categoryId,
        string description,
        string extTransactionNo,
        bool isPaidToOwnAccount,
        PayerPayeeId paidToId,
        //RefTransactionId refTransactionId,
        SubCategoryId subCategoryId,
        IEnumerable<string>? tags,
        DateTime transactionDate)
    {
        return new PaymentOutTransaction(
            userId,
            accountId,
            description,
            extTransactionNo,
            isPaidToOwnAccount,
            TransactionItem.New(
                new Money(amount),
                categoryId,
                subCategoryId,
                description),
            paidToId,
            tags,
            transactionDate);
    }
}

