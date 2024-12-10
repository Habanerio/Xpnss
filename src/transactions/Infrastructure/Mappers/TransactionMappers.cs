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
    /// Maps an individual <see cref="TransactionDocument"/> to a <see cref="TransactionBase"/>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static TransactionBase? Map(TransactionDocument? document)
    {
        if (document == null)
            return default;

        if (document is PurchaseTransactionDocument purchaseDocument)
            return Map(purchaseDocument);

        // Transfer Document
        // ...

        if (document is CreditTransactionDocument creditDocument)
            return Map(creditDocument);

        if (document is DebitTransactionDocument debitDocument)
            return Map(debitDocument);

        throw new InvalidOperationException("Transaction document type not supported");
    }

    public static CreditTransaction? Map(CreditTransactionDocument? document)
    {
        if (document == null)
            return default;

        if (!document.IsCredit)
            throw new InvalidOperationException($"Transaction Document is not a Credit Document." +
                $"{Environment.NewLine}Transaction Id: {document.Id}" +
                $"{Environment.NewLine}Transaction Type: {document.TransactionType}");

        var transactionItem = Map(document.Items[0]) ??
            throw new InvalidOperationException("TransactionItem is null");

        var transaction = CreditTransaction.Load(
            new TransactionId(document.Id.ToString()),
            new UserId(document.UserId),
            new AccountId(document.AccountId.ToString()),
            document.Description,
            document.ExtTransactionId,
            transactionItem,
            new PayerPayeeId(document.PayerPayeeId),
            document.Tags,
            document.TransactionDate,
            document.TransactionType,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);

        return transaction;
    }

    public static DebitTransaction? Map(DebitTransactionDocument? document)
    {
        if (document == null)
            return default;

        if (document.IsCredit)
            throw new InvalidOperationException($"Transaction Document is not a Debit Document." +
                $"{Environment.NewLine}Transaction Id: {document.Id}" +
                $"{Environment.NewLine}Transaction Type: {document.TransactionType}");

        var transactionItem = Map(document.Items[0]) ??
            throw new InvalidOperationException("TransactionItem is null");

        var transaction = DebitTransaction.Load(
            new TransactionId(document.Id.ToString()),
            new UserId(document.UserId),
            new AccountId(document.AccountId.ToString()),
            document.Description,
            document.ExtTransactionId,
            transactionItem,
            new PayerPayeeId(document.PayerPayeeId),
            document.Tags,
            document.TransactionDate,
            document.TransactionType,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);

        return transaction;
    }

    //public static CreditTransaction? Map(DepositTransactionDocument? depositDocument)
    //{
    //    if (depositDocument == null)
    //        return default;

    //    if (!depositDocument.TransactionType.Equals(TransactionEnums.TransactionKeys.DEPOSIT))
    //        throw new InvalidOperationException("Transaction document is not a DepositTransactionDocument");

    //    var transactionItem = Map(depositDocument.Items[0]) ??
    //        throw new InvalidOperationException("TransactionItem is null");

    //    var transaction = CreditTransaction.Load(
    //        new TransactionId(depositDocument.Id.ToString()),
    //        new UserId(depositDocument.UserId),
    //        new AccountId(depositDocument.AccountId.ToString()),
    //        depositDocument.Description,
    //        depositDocument.ExtTransactionId,
    //        transactionItem,
    //        new PayerPayeeId(depositDocument.PayerPayeeId),
    //        depositDocument.Tags,
    //        depositDocument.TransactionDate,
    //        depositDocument.TransactionType,
    //        depositDocument.DateCreated,
    //        depositDocument.DateUpdated,
    //        depositDocument.DateDeleted);

    //    return transaction;
    //}

    /// <summary>
    /// Maps an individual <see cref="PurchaseTransactionDocument"/> to a <see cref="PurchaseTransaction"/>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static PurchaseTransaction? Map(PurchaseTransactionDocument? purchaseDocument)
    {
        if (purchaseDocument == null)
            return default;

        if (!purchaseDocument.TransactionType.Equals(TransactionEnums.TransactionKeys.PURCHASE))
            throw new InvalidOperationException("Transaction document is not a PurchaseTransactionDocument");

        var purchaseItems = Map(purchaseDocument.Items);

        var transaction = PurchaseTransaction.Load(
            new TransactionId(purchaseDocument.Id.ToString()),
            new UserId(purchaseDocument.UserId),
            new AccountId(purchaseDocument.AccountId.ToString()),
            purchaseDocument.Description,
            new PayerPayeeId(purchaseDocument.PayerPayeeId?.ToString()),
            purchaseItems,
            purchaseDocument.TransactionDate,
            purchaseDocument.Tags,
            purchaseDocument.ExtTransactionId,
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
    public static IEnumerable<TransactionBase> Map(IEnumerable<TransactionDocument> documents)
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




    public static TransactionDocument? Map(TransactionBase? transaction)
    {
        if (transaction is null)
            return default;

        if (transaction is PurchaseTransaction purchaseTransaction)
            return Map(purchaseTransaction);

        if (transaction is CreditTransaction creditTransaction)
            return Map(creditTransaction);

        if (transaction is DebitTransaction debitTransaction)
            return Map(debitTransaction);


        throw new InvalidOperationException("Transaction type not supported");
    }

    /// <summary>
    /// Maps a `DepositTransaction` to a `DepositTransactionDocument`
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">When Transaction is not a DepositTransaction</exception>
    public static CreditTransactionDocument? Map(CreditTransaction? transaction)
    {
        if (transaction is null)
            return default;

        if (!transaction.IsCredit)
            throw new InvalidOperationException("Transaction is not a CreditTransaction");

        var document = new CreditTransactionDocument(transaction.TransactionType)
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            AccountId = transaction.AccountId,
            TransactionType = transaction.TransactionType,
            Description = transaction.Description,
            ExtTransactionId = transaction.ExtTransactionId,
            IsDeleted = transaction.IsDeleted,
            Items = Map(transaction.Items),
            PayerPayeeId = transaction.PayerPayeeId,
            //PayerPayeeId = !string.IsNullOrWhiteSpace(transaction.PayerPayeeId) ?
            //    ObjectId.Parse(transaction.PayerPayeeId.Value) :
            //    null,
            //PayerPayeeId = ObjectId.TryParse(transaction.PayerPayeeId, out var payerPayeeObjectId) ? payerPayeeObjectId : null,
            Tags = transaction.Tags.ToList(),
            TransactionDate = transaction.TransactionDate,

            DateCreated = transaction.DateCreated,
            DateUpdated = transaction.DateUpdated,
            DateDeleted = transaction.DateDeleted
        };

        return document;
    }

    /// <summary>
    /// Maps a `DepositTransaction` to a `DepositTransactionDocument`
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">When Transaction is not a DepositTransaction</exception>
    public static DebitTransactionDocument? Map(DebitTransaction? transaction)
    {
        if (transaction is null)
            return default;

        if (transaction.IsCredit)
            throw new InvalidOperationException("Transaction is not a DebitTransaction");

        var document = new DebitTransactionDocument(transaction.TransactionType)
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            AccountId = transaction.AccountId,
            TransactionType = transaction.TransactionType,
            Description = transaction.Description,
            ExtTransactionId = transaction.ExtTransactionId,
            IsDeleted = transaction.IsDeleted,
            PayerPayeeId = transaction.PayerPayeeId,
            //PayerPayeeId = !string.IsNullOrWhiteSpace(transaction.PayerPayeeId) ?
            //    ObjectId.Parse(transaction.PayerPayeeId.Value) :
            //    null,
            //PayerPayeeId = ObjectId.TryParse(transaction.PayerPayeeId, out var payerPayeeObjectId) ? payerPayeeObjectId : null,
            Tags = transaction.Tags.ToList(),
            TransactionDate = transaction.TransactionDate,

            DateCreated = transaction.DateCreated,
            DateUpdated = transaction.DateUpdated,
            DateDeleted = transaction.DateDeleted
        };

        return document;
    }

    /// <summary>
    /// Maps a `PurchaseTransaction` to a `PurchaseTransactionDocument`
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">When Transaction is not a PurchaseTransaction</exception>
    public static PurchaseTransactionDocument? Map(PurchaseTransaction? transaction)
    {
        if (transaction is null)
            return default;

        if (!transaction.TransactionType.Equals(TransactionEnums.TransactionKeys.PURCHASE))
            throw new InvalidOperationException($"{nameof(transaction)} is not a valid {nameof(PurchaseTransaction)}");

        var document = new PurchaseTransactionDocument
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            AccountId = transaction.AccountId,
            TransactionType = transaction.TransactionType,
            Description = transaction.Description,
            ExtTransactionId = transaction.ExtTransactionId,
            IsDeleted = transaction.IsDeleted,
            Items = Map(transaction.Items).ToList(),
            PayerPayeeId = transaction.PayerPayeeId,
            //PayerPayeeId = !string.IsNullOrWhiteSpace(transaction.PayerPayeeId) ?
            //    ObjectId.Parse(transaction.PayerPayeeId.Value) :
            //    null,
            Payments = Map(transaction.Payments).ToList(),
            Tags = transaction.Tags.ToList(),
            TransactionDate = transaction.TransactionDate,

            DateCreated = transaction.DateCreated,
            DateUpdated = transaction.DateUpdated,
            DateDeleted = transaction.DateDeleted
        };

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