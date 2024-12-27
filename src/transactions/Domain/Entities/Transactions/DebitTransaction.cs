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
            string extTransactionNo,
            TransactionItem item,
            PayerPayeeId payerPayeeId,
            IEnumerable<string>? tags,
            string title,
            DateTime transactionDate,
            TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            description,
            extTransactionNo,
            isCredit: false,
            item,
            payerPayeeId,
            tags,
            title,
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
        string extTransactionNo,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        string title,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            description,
            extTransactionNo,
            isCredit: false,
            items,
            payerPayeeId,
            //refTransactionId,
            tags,
            title,
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
            string extTransactionNo,
            TransactionItem item,
            PayerPayeeId payerPayeeId,
            //RefTransactionId refTransactionId,
            IEnumerable<string>? tags,
            string title,
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
            extTransactionNo,
            isCredit: false,
            item,
            payerPayeeId,
            //refTransactionId,
            tags,
            title,
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
        string extTransactionNo,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        IEnumerable<string>? tags,
        string title,
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
            extTransactionNo,
            isCredit: false,
            items,
            payerPayeeId,
            tags,
            title,
            transactionDate,
            transactionType,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }

    public static DebitTransaction New(
        UserId userId,
        TransactionEnums.TransactionKeys transactionType,
        AccountId accountId,
        Money amount,
        CategoryId categoryId,
        string description,
        PayerPayeeId payerPayeeId,
        SubCategoryId subCategoryId,
        string title,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionNo = "")
    {
        return new DebitTransaction(
            userId,
            accountId,
            description,
            extTransactionNo,
            TransactionItem.New(amount, categoryId, subCategoryId, description),
            payerPayeeId,
            tags,
            title,
            transactionDate,
            transactionType: transactionType);
    }

    public static DebitTransaction NewWithdrawal(
        UserId userId,
        AccountId accountId,
        Money amount,
        CategoryId categoryId,
        string description,
        PayerPayeeId payerPayeeId,
        SubCategoryId subCategoryId,
        string title,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionNo = "")
    {
        return new DebitTransaction(
            userId,
            accountId,
            description,
            extTransactionNo,
            TransactionItem.New(amount, categoryId, subCategoryId, description),
            payerPayeeId,
            tags,
            title,
            transactionDate,
            transactionType: TransactionEnums.TransactionKeys.WITHDRAWAL);
    }


    //// Payment may be its own transaction type in the future
    //// As a payment can go from one Account to another
    //public static DebitTransaction NewPayment(
    //    UserId userId,
    //    AccountId accountId,
    //    Money amount,
    //    CategoryId categoryId,
    //    string description,
    //    string extTransactionNo,
    //    PayerPayeeId paidTo,
    //    SubCategoryId subCategoryId,
    //    IEnumerable<string>? tags,
    //    DateTime transactionDate)
    //{
    //    return new DebitTransaction(
    //        userId,
    //        accountId,
    //        description,
    //        extTransactionNo,
    //        TransactionItem.New(amount, categoryId, subCategoryId, description),
    //        paidTo,
    //        tags,
    //        transactionDate,
    //        transactionType: TransactionEnums.TransactionKeys.PAYMENT);
    //}

    public static DebitTransaction Load(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        CategoryId categoryId,
        string description,
        string extTransactionNo,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        SubCategoryId subCategoryId,
        IEnumerable<string>? tags,
        string title,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        if (TransactionEnums.IsCreditTransaction(transactionType))
            throw new InvalidOperationException($"'{transactionType.ToString()}': Transaction type is not a Debit Transaction");

        return new DebitTransaction(
            id,
            userId,
            accountId,
            description,
            extTransactionNo,
            item,
            payerPayeeId,
            //refTransactionId,
            tags,
            title,
            transactionDate,
            transactionType,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }
}