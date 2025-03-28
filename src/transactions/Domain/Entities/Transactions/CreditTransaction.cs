using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public class CreditTransaction : Transaction
{
    /// <summary>
    /// New with one Item
    /// </summary>
    protected CreditTransaction(
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
            TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            description,
            extTransactionNo,
            isCredit: true,
            item,
            payerPayeeId,
            //refTransactionId,
            tags,
            title,
            transactionDate,
            transactionType)
    { }

    /// <summary>
    /// Existing with a single Item
    /// </summary>
    protected CreditTransaction(
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
            isCredit: true,
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

    public static CreditTransaction New(
        UserId userId,
        TransactionEnums.TransactionKeys transactionType,
        AccountId accountId,
        Money amount,
        CategoryId categoryId,
        string description,
        string extTransactionNo,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        SubCategoryId subCategoryId,
        string title,
        IEnumerable<string>? tags,
        DateTime transactionDate)
    {
        return new CreditTransaction(
            userId,
            accountId,
            description,
            extTransactionNo,
            TransactionItem.New(
                new Money(amount),
                categoryId,
                subCategoryId,
                description),
            payerPayeeId,
            //refTransactionId,
            tags,
            title,
            transactionDate,
            transactionType);
    }

    public static CreditTransaction NewDeposit(
        UserId userId,
        AccountId accountId,
        Money amount,
        CategoryId categoryId,
        string description,
        string extTransactionNo,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        SubCategoryId subCategoryId,
        IEnumerable<string>? tags,
        string title,
        DateTime transactionDate)
    {
        return new CreditTransaction(
            userId,
            accountId,
            description,
            extTransactionNo,
            TransactionItem.New(
                new Money(amount),
                categoryId,
                subCategoryId,
                description),
            payerPayeeId,
            //refTransactionId,
            tags,
            title,
            transactionDate,
            TransactionEnums.TransactionKeys.DEPOSIT);
    }


    public static CreditTransaction Load(
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
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        if (!TransactionEnums.IsCreditTransaction(transactionType))
            throw new InvalidOperationException($"'{transactionType.ToString()}' " +
                                                $"Transaction type is not a Credit Transaction");

        return new CreditTransaction(
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