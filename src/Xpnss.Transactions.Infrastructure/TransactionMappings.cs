using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Transactions.Domain;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Transactions.Infrastructure;

internal static partial class Mapper
{
    /// <summary>
    /// Maps an individual <see cref="TransactionDocument"/> to a <see cref="Transaction"/>
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public static Transaction? Map(TransactionDocument? document)
    {
        if (document == null)
            return null;

        var transaction = Transaction.Load(
            new TransactionId(document.Id.ToString()),
            new UserId(document.UserId),
            new AccountId(document.AccountId.ToString()),
            document.TransactionType,
            document.Description,
            document.MerchantId is null ?
                MerchantId.New :
                new MerchantId(document.MerchantId.ToString()!),
            Map(document.Items),
            Map(document.Payments),
            document.TransactionDate,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);

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
            return null;

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

    public static TransactionPayment? Map(TransactionDocumentPayment? document)
    {
        if (document is null)
            return null;

        return TransactionPayment.Load(
            new TransactionPaymentId(document.Id),
             new Money(document.Amount),
             document.PaymentDate);
    }

    public static IEnumerable<TransactionPayment> Map(IEnumerable<TransactionDocumentPayment> documents)
        => documents.Select(Map).Where(t => t is not null).Select(t => t!);

    public static TransactionDocument? Map(Transaction? transaction)
    {
        if (transaction is null)
            return null;

        var document = new TransactionDocument
        {
            Id = ObjectId.Parse(transaction.Id),
            UserId = transaction.UserId,
            AccountId = ObjectId.Parse(transaction.AccountId),
            TransactionType = transaction.TransactionType,
            Description = transaction.Description,
            MerchantId = string.IsNullOrWhiteSpace(transaction.MerchantId) ?
                null :
                ObjectId.Parse(transaction.MerchantId).ToString(),
            Items = Map(transaction.Items).ToList(),
            Payments = Map(transaction.Payments).ToList(),
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
            return null;

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

    public static TransactionDocumentPayment? Map(TransactionPayment? payment)
    {
        if (payment is null)
            return null;

        var paymentDoc = new TransactionDocumentPayment(
            ObjectId.Parse(payment.Id),
            payment.Amount,
            payment.PaymentDate);

        return paymentDoc;
    }

    public static IEnumerable<TransactionDocumentPayment> Map(IEnumerable<TransactionPayment> payments)
        => payments.Select(Map).Where(t => t is not null).Select(t => t!);
}