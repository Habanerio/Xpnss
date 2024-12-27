using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;
using Habanerio.Xpnss.Transactions.Domain.Entities;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Infrastructure.Data.Documents;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Transactions.Infrastructure.Mappers;

internal static partial class InfrastructureMapper
{
    /// <summary>
    /// Maps an individual <see cref="TransactionDocument"/> to a <see cref="Transaction"/>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static Transaction? Map(TransactionDocument? document)
    {
        if (document == null)
            return default;

        if (document is PurchasesTransactionDocument purchaseDocument)
            return Map(purchaseDocument);

        // Internally, all transactions have at least one underlying item
        var transactionItem = Map(document.Items[0]) ??
            throw new InvalidOperationException("TransactionItem is null");

        if (document is PaymentInTransactionDocument paymentInDocument)
        {
            if (paymentInDocument.TransactionType.Equals(TransactionEnums.TransactionKeys.PAYMENT_IN))
            {
                var transaction = new PaymentInTransaction(
                    new TransactionId(paymentInDocument.Id),
                    new UserId(paymentInDocument.UserId),
                    new AccountId(paymentInDocument.AccountId),
                    paymentInDocument.Description,
                    paymentInDocument.ExtTransactionNo,
                    transactionItem,
                    new PayerPayeeId(paymentInDocument.PayerPayeeId),
                    paymentInDocument.IsPaidFromOwnAccount,
                    paymentInDocument.Tags,
                    paymentInDocument.Title,
                    paymentInDocument.TransactionDate,
                    paymentInDocument.DateCreated,
                    paymentInDocument.DateUpdated,
                    paymentInDocument.DateDeleted);

                return transaction;
            }

            throw new InvalidOperationException("Invalid Payment Type");
        }


        if (document is CreditTransactionDocument creditDocument)
        {
            var transaction = CreditTransaction.Load(
                new TransactionId(creditDocument.Id.ToString()),
                new UserId(creditDocument.UserId),
                new AccountId(creditDocument.AccountId),
                creditDocument.Description,
                creditDocument.ExtTransactionNo,
                transactionItem,
                new PayerPayeeId(creditDocument.PayerPayeeId),
                //new RefTransactionId(creditDocument.RefTransactionId),
                creditDocument.Tags,
                creditDocument.Title,
                creditDocument.TransactionDate,
                creditDocument.TransactionType,
                creditDocument.DateCreated,
                creditDocument.DateUpdated,
                creditDocument.DateDeleted);

            return transaction;
        }

        if (document is PaymentOutTransactionDocument paymentOutDocument)
        {
            if (paymentOutDocument.TransactionType.Equals(TransactionEnums.TransactionKeys.PAYMENT_OUT))
            {
                var transaction = new PaymentOutTransaction(
                    new TransactionId(paymentOutDocument.Id),
                    new UserId(paymentOutDocument.UserId),
                    new AccountId(paymentOutDocument.AccountId),
                    paymentOutDocument.Description,
                    paymentOutDocument.ExtTransactionNo,
                    paymentOutDocument.IsPaidToOwnAccount,
                    transactionItem,
                    new PayerPayeeId(paymentOutDocument.PayerPayeeId),
                    paymentOutDocument.Tags,
                    paymentOutDocument.Title,
                    paymentOutDocument.TransactionDate,
                    paymentOutDocument.DateCreated,
                    paymentOutDocument.DateUpdated,
                    paymentOutDocument.DateDeleted);

                return transaction;
            }

            throw new InvalidOperationException("Invalid Payment Type");
        }

        if (document is DebitTransactionDocument debitDocument)
        {
            var transaction = DebitTransaction.Load(
                new TransactionId(debitDocument.Id.ToString()),
                new UserId(debitDocument.UserId),
                new AccountId(debitDocument.AccountId),
                new CategoryId(debitDocument.CategoryId),
                debitDocument.Description,
                debitDocument.ExtTransactionNo,
                transactionItem,
                new PayerPayeeId(debitDocument.PayerPayeeId),
                //new RefTransactionId(debitDocument.RefTransactionId),
                new SubCategoryId(debitDocument.SubCategoryId),
                debitDocument.Tags,
                debitDocument.Title,
                debitDocument.TransactionDate,
                debitDocument.TransactionType,
                debitDocument.DateCreated,
                debitDocument.DateUpdated,
                debitDocument.DateDeleted);

            return transaction;
        }

        throw new InvalidOperationException("TransactionMapper");
    }

    /// <summary>
    /// Maps an individual <see cref="PurchasesTransactionDocument"/> to a <see cref="PurchasesTransaction"/>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static PurchasesTransaction? Map(PurchasesTransactionDocument? purchaseDocument)
    {
        if (purchaseDocument == null)
            return default;

        if (!purchaseDocument.TransactionType.Equals(TransactionEnums.TransactionKeys.PURCHASE))
            throw new InvalidOperationException("Transaction document is not a PurchasesTransactionDocument");

        var purchaseItems = Map(purchaseDocument.Items);

        var transaction = PurchasesTransaction.Load(
            new TransactionId(purchaseDocument.Id),
            new UserId(purchaseDocument.UserId),
            new AccountId(purchaseDocument.AccountId),
            purchaseDocument.Description,
            purchaseDocument.ExtTransactionNo,
            purchaseItems,
            new PayerPayeeId(purchaseDocument.PayerPayeeId),
            // new RefTransactionId(purchaseDocument.RefTransactionId),
            purchaseDocument.Tags,
            purchaseDocument.Title,
            purchaseDocument.TransactionDate,
            purchaseDocument.DateCreated,
            purchaseDocument.DateUpdated,
            purchaseDocument.DateDeleted);

        return transaction;
    }


    /// <summary>
    /// Maps a collection of <see cref="TransactionDocument"/> to a collection of <see cref="Transaction"/>
    /// Use this instead of manually iterating over the collection and calling <see cref="Map(TransactionDocument)"/>
    /// </summary>
    /// <param name="documents">The collection of documents to convert to domain entities</param>
    /// <returns></returns>
    public static IEnumerable<Transaction> Map(IEnumerable<TransactionDocument> documents)
        => documents.Select(Map).Where(t => t is not null).Select(t => t!);


    /// <summary>
    /// Maps an individual <see cref="TransactionDocumentItem"/> to a <see cref="TransactionItem"/>
    /// </summary>
    /// <param name="document">The document to convert to a domain entity</param>
    /// <returns></returns>
    public static TransactionItem? Map(TransactionDocumentItem? document)
    {
        if (document is null)
            return default;

        return TransactionItem.Load(
            new TransactionItemId(document.Id),
            new Money(document.Amount),
            new CategoryId(document.CategoryId),
            new SubCategoryId(document.SubCategoryId),
            document.Description);
    }

    /// <summary>
    /// Maps a collection of <see cref="TransactionDocumentItem"/> to a collection of <see cref="TransactionItem"/>
    /// Use this instead of manually iterating over the collection and calling <see cref="Map(TransactionDocumentItem)"/>
    /// </summary>
    /// <param name="documents"></param>
    /// <returns></returns>
    public static IEnumerable<TransactionItem> Map(IEnumerable<TransactionDocumentItem> documents)
        => documents.Select(Map).Where(t => t is not null).Select(t => t!);


    public static TransactionPaymentItem? Map(TransactionDocumentPayment? document)
    {
        if (document is null)
            return default;

        return TransactionPaymentItem.Load(
            new TransactionPaymentId(document.Id),
             new Money(document.Amount),
             document.PaymentDate);
    }

    public static IEnumerable<TransactionPaymentItem> Map(IEnumerable<TransactionDocumentPayment> documents)
        => documents.Select(Map).Where(t => t is not null).Select(t => t!);


    public static TransactionDocument? Map(Transaction? transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction is PaymentOutTransaction paymentMadeTransaction)
        {
            return MapCommonProperties<PaymentOutTransactionDocument>(paymentMadeTransaction);
        }

        if (transaction is PaymentInTransaction paymentReceivedTransaction)
        {
            return MapCommonProperties<PaymentInTransactionDocument>(paymentReceivedTransaction);
        }

        if (transaction is PurchasesTransaction purchasesTransaction)
            return MapCommonProperties<PurchasesTransactionDocument>(purchasesTransaction);

        if (transaction is CreditTransaction creditTransaction)
            return MapCommonProperties<CreditTransactionDocument>(creditTransaction);

        if (transaction is DebitTransaction debitTransaction)
            return MapCommonProperties<DebitTransactionDocument>(debitTransaction);

        throw new InvalidOperationException($"InfrastructureMapper.Transaction: Transaction type not supported");
    }

    private static TDoc MapCommonProperties<TDoc>(Transaction? transaction)
        where TDoc : TransactionDocument, new()
    {
        if (transaction is null)
            return default;

        var document = new TDoc
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            AccountId = transaction.AccountId,
            CategoryId = transaction.CategoryId,
            Description = transaction.Description,
            ExtTransactionNo = transaction.ExtTransactionNo,
            IsCredit = transaction.IsCredit,
            Items = Map(transaction.Items),
            PayerPayeeId = transaction.PayerPayeeId,
            //RefTransactionId = transaction.RefTransactionId,
            Tags = transaction.Tags.ToList(),
            Title = transaction.Title,
            TransactionDate = transaction.TransactionDate,
            SubCategoryId = transaction.SubCategoryId,
            TransactionType = transaction.TransactionType,

            DateCreated = transaction.DateCreated,
            DateUpdated = transaction.DateUpdated,
            DateDeleted = transaction.DateDeleted
        };

        if (document is PaymentInTransactionDocument paymentInDocument &&
            transaction is PaymentTransaction paymentInTransaction)
        {
            paymentInDocument.IsPaidFromOwnAccount = paymentInTransaction.IsExistingAccount;
        }

        if (document is PaymentInTransactionDocument paymentOutDocument &&
            transaction is PaymentTransaction paymentOutTransaction)
        {
            paymentOutDocument.IsPaidFromOwnAccount = paymentOutTransaction.IsExistingAccount;
        }

        if (document is PurchasesTransactionDocument purchaseDocument &&
            transaction is PurchasesTransaction purchasesTransaction)
        {
            purchaseDocument.PaidDate = purchasesTransaction.PaidDate;
        }

        return document;
    }

    /// <summary>
    /// Maps a TransactionItem to a TransactionDocumentItem
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static TransactionDocumentItem? Map(TransactionItem? item)
    {
        if (item is null)
            return default;

        var itemDoc = new TransactionDocumentItem(
            ObjectId.Parse(item.Id),
            item.Amount,
            string.IsNullOrWhiteSpace(item.CategoryId) ?
                null :
                ObjectId.Parse(item.CategoryId),
            string.IsNullOrWhiteSpace(item.SubCategoryId) ?
            null :
            ObjectId.Parse(item.SubCategoryId),
            item.Description);

        return itemDoc;
    }

    public static List<TransactionDocumentItem> Map(IEnumerable<TransactionItem> items)
        => items.Select(Map)
            .Where(t => t is not null)
            .Select(t => t!)
        .ToList();

    public static TransactionDocumentPayment? Map(TransactionPaymentItem? payment)
    {
        if (payment is null)
            return default;

        var paymentDoc = new TransactionDocumentPayment(
            ObjectId.Parse(payment.Id),
            payment.Amount,
            payment.PaymentDate);

        return paymentDoc;
    }

    public static IEnumerable<TransactionDocumentPayment> Map(IEnumerable<TransactionPaymentItem> payments)
        => payments.Select(Map).Where(t => t is not null).Select(t => t!);
}