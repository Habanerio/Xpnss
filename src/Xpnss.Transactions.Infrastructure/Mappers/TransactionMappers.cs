using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;
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

        if (document is DepositTransactionDocument depositDocDocument)
            return Map(depositDocDocument);

        if (document is PurchaseTransactionDocument purchaseDocument)
            return Map(purchaseDocument);

        throw new InvalidOperationException("Transaction document type not supported");
    }

    public static DepositTransaction? Map(DepositTransactionDocument? depositDocument)
    {
        if (depositDocument == null)
            return default;

        if (!depositDocument.TransactionType.Equals(TransactionTypes.Keys.DEPOSIT))
            throw new InvalidOperationException("Transaction document is not a DepositTransactionDocument");

        var transaction = DepositTransaction.Load(
            new TransactionId(depositDocument.Id.ToString()),
            new UserId(depositDocument.UserId),
            new AccountId(depositDocument.AccountId.ToString()),
            new Money(depositDocument.TotalAmount),
            depositDocument.Description,
            new PayerPayeeId(depositDocument.PayerPayeeId),
            depositDocument.TransactionDate,
            depositDocument.Tags,

            depositDocument.DateCreated,
            depositDocument.DateUpdated,
            depositDocument.DateDeleted);

        return transaction;
    }

    /// <summary>
    /// Maps an individual <see cref="PurchaseTransactionDocument"/> to a <see cref="PurchaseTransaction"/>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static PurchaseTransaction? Map(PurchaseTransactionDocument? purchaseDocument)
    {
        if (purchaseDocument == null)
            return default;

        if (!purchaseDocument.TransactionType.Equals(TransactionTypes.Keys.PURCHASE))
            throw new InvalidOperationException("Transaction document is not a PurchaseTransactionDocument");

        var purchaseItems = Map(purchaseDocument.Items);

        var transaction = PurchaseTransaction.Load(
            new TransactionId(purchaseDocument.Id.ToString()),
            new UserId(purchaseDocument.UserId),
            new AccountId(purchaseDocument.AccountId.ToString()),
            purchaseDocument.Description,
            new PayerPayeeId(purchaseDocument.PayerPayeeId?.ToString()),
            purchaseItems,
            Map(purchaseDocument.Payments),
            purchaseDocument.TransactionDate,
            purchaseDocument.Tags,
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

        // Debits
        if (transaction is DepositTransaction depositTransaction)
            return Map(depositTransaction);


        // Credits
        if (transaction is PurchaseTransaction purchaseTransaction)
            return Map(purchaseTransaction);


        throw new InvalidOperationException("Transaction type not supported");
    }

    /// <summary>
    /// Maps a `DepositTransaction` to a `DepositTransactionDocument`
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">When Transaction is not a DepositTransaction</exception>
    public static DepositTransactionDocument? Map(DepositTransaction? transaction)
    {
        if (transaction is null)
            return default;

        if (!transaction.TransactionType.Equals(TransactionTypes.Keys.DEPOSIT))
            throw new InvalidOperationException("Transaction is not a DepositTransaction");

        //var transactionId = transaction.Id;
        //ObjectId? payerPayeeId = transaction.PayerPayeeId?.Value is not null ? 
        //    ObjectId.Parse(transaction.PayerPayeeId.Value) : 
        //    null;

        var document = new DepositTransactionDocument
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            AccountId = transaction.AccountId,
            TransactionType = transaction.TransactionType,
            Description = transaction.Description,
            IsDeleted = transaction.IsDeleted,
            PayerPayeeId = !string.IsNullOrWhiteSpace(transaction.PayerPayeeId) ?
                ObjectId.Parse(transaction.PayerPayeeId.Value) :
                null,
            //PayerPayeeId = ObjectId.TryParse(transaction.PayerPayeeId, out var payerPayeeObjectId) ? payerPayeeObjectId : null,
            Tags = transaction.Tags.ToList(),
            TotalAmount = transaction.TotalAmount,
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

        if (!transaction.TransactionType.Equals(TransactionTypes.Keys.PURCHASE))
            throw new InvalidOperationException("Transaction is not a PurchaseTransaction");

        var document = new PurchaseTransactionDocument
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            AccountId = transaction.AccountId,
            TransactionType = transaction.TransactionType,
            Description = transaction.Description,
            IsDeleted = transaction.IsDeleted,
            Items = Map(transaction.Items).ToList(),
            PayerPayeeId = !string.IsNullOrWhiteSpace(transaction.PayerPayeeId) ?
                ObjectId.Parse(transaction.PayerPayeeId.Value) :
                null,
            Payments = Map(transaction.Payments).ToList(),
            Tags = transaction.Tags.ToList(),
            TotalAmount = transaction.TotalAmount,
            TotalOwing = transaction.TotalOwing,
            TotalPaid = transaction.TotalPaid,
            TransactionDate = transaction.TransactionDate,

            DateCreated = transaction.DateCreated,
            DateUpdated = transaction.DateUpdated,
            DateDeleted = transaction.DateDeleted
        };

        return document;
    }


    public static TransactionDocumentItem? Map(TransactionItem? item)
    {
        if (item is null)
            return default;

        var itemDoc = new TransactionDocumentItem(
            ObjectId.Parse(item.Id),
            item.Amount,
            string.IsNullOrWhiteSpace(item.CategoryId) ?
                null :
                ObjectId.Parse(item.CategoryId), item.Description);

        return itemDoc;
    }

    public static IEnumerable<TransactionDocumentItem> Map(IEnumerable<TransactionItem> items)
        => items.Select(Map).Where(t => t is not null).Select(t => t!);

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