using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Transactions.Infrastructure.Data.Documents;

[BsonCollection("money_accounts_transactions")]
public class TransactionDocument : MongoDocument
{
    [BsonElement("user_id")]
    public ObjectId UserId { get; set; }

    [BsonElement("account_id")]
    public ObjectId AccountId { get; set; }

    [BsonElement("transaction_type")]
    [BsonRepresentation(BsonType.String)]
    public TransactionEnums.TransactionKeys TransactionType { get; set; }

    [BsonElement("category_id")]
    public ObjectId? CategoryId { get; set; }

    [BsonElement("sub_category_id")]
    public ObjectId? SubCategoryId { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = "";

    [BsonElement("ext_transaction_id")]
    public string ExtTransactionNo { get; set; } = "";

    [BsonElement("is_credit")]
    public bool IsCredit { get; set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted => DateDeleted.HasValue;

    [BsonElement("items")]
    public List<TransactionDocumentItem> Items { get; set; } = [];

    [BsonElement("payerpayee_id")]
    public ObjectId? PayerPayeeId { get; set; }

    /// <summary>
    /// Reference to another transaction within the system
    /// </summary>
    [BsonElement("ref_transaction_id")]
    public ObjectId? RefTransactionId { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = [];

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("total_amount")]
    public decimal TotalAmount => Items.Sum(i => i.Amount);

    [BsonElement("transaction_date")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime TransactionDate { get; set; }

    [BsonElement("date_created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateCreated { get; set; }

    [BsonElement("date_updated")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateUpdated { get; set; } = null;

    [BsonElement("date_deleted")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateDeleted { get; set; } = null;

    public TransactionDocument()
    {
        Id = ObjectId.GenerateNewId();
    }

    public TransactionDocument(bool isCredit, TransactionEnums.TransactionKeys transactionType)
    {
        IsCredit = isCredit;
        TransactionType = transactionType;
    }
}


// Credits
public class CreditTransactionDocument :
    TransactionDocument
{
    public CreditTransactionDocument() :
        base(isCredit: true, transactionType: TransactionEnums.TransactionKeys.DEPOSIT)
    { }

    public CreditTransactionDocument(TransactionEnums.TransactionKeys transactionType) :
        base(isCredit: true, transactionType)
    { }
}


// Debits
public class DebitTransactionDocument :
    TransactionDocument
{
    public DebitTransactionDocument() :
        base(isCredit: false, transactionType: TransactionEnums.TransactionKeys.WITHDRAWAL)
    { }

    public DebitTransactionDocument(TransactionEnums.TransactionKeys transactionType) :
        base(isCredit: false, transactionType)
    { }
}

public class PaymentTransactionDocument :
    TransactionDocument
{
    /// <summary>
    /// If the transaction is from the user's own account
    /// </summary>
    public bool IsOwnAccount { get; set; }

    public PaymentTransactionDocument(
        bool isCredit,
        TransactionEnums.TransactionKeys transactionType) :
        base(isCredit, transactionType)
    { }

    public PaymentTransactionDocument(
        ObjectId userId,
        ObjectId accountId,
        CategoryId categoryId,
        string description,
        string extTransactionNo,
        bool isCredit,
        bool isOwnAccount,
        TransactionDocumentItem item,
        PayerPayeeId payerPayeeId,
        SubCategoryId subCategoryId,
        IEnumerable<string>? tags,
        string title,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) :
        base(isCredit, transactionType)
    {
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Description = description;
        ExtTransactionNo = extTransactionNo;
        IsOwnAccount = isOwnAccount;
        Items = [item];
        PayerPayeeId = payerPayeeId;
        SubCategoryId = subCategoryId;
        Tags = tags?.ToList() ?? [];
        Title = title;
        TransactionDate = transactionDate;
        TransactionType = TransactionEnums.TransactionKeys.PAYMENT_IN;

        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;
    }
}

public class PaymentInTransactionDocument :
    CreditTransactionDocument
{
    /// <summary>
    /// If the transaction is from the user's own account
    /// </summary>
    public bool IsPaidFromOwnAccount { get; set; }

    public PaymentInTransactionDocument() :
        base(TransactionEnums.TransactionKeys.PAYMENT_IN)
    { }

    public PaymentInTransactionDocument(
        ObjectId userId,
        ObjectId accountId,
        CategoryId categoryId,
        string description,
        string extTransactionNo,
        bool isPaidFromOwnAccount,
        TransactionDocumentItem item,
        PayerPayeeId payerPayeeId,
        SubCategoryId subCategoryId,
        IEnumerable<string>? tags,
        string title,
        DateTime transactionDate,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) :
        base(TransactionEnums.TransactionKeys.PAYMENT_IN)
    {
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Description = description;
        ExtTransactionNo = extTransactionNo;
        IsPaidFromOwnAccount = isPaidFromOwnAccount;
        Items = [item];
        PayerPayeeId = payerPayeeId;
        SubCategoryId = subCategoryId;
        Tags = tags?.ToList() ?? [];
        Title = title;
        TransactionDate = transactionDate;
        TransactionType = TransactionEnums.TransactionKeys.PAYMENT_IN;

        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;
    }
}

public class PaymentOutTransactionDocument :
    DebitTransactionDocument
{
    /// <summary>
    /// If the transaction is to the user's own account
    /// </summary>
    public bool IsPaidToOwnAccount { get; set; }

    public PaymentOutTransactionDocument() :
        base(transactionType: TransactionEnums.TransactionKeys.PAYMENT_OUT)
    { }

    public PaymentOutTransactionDocument(
        ObjectId userId,
        ObjectId accountId,
        CategoryId categoryId,
        string description,
        string extTransactionNo,
        bool isPaidToOwnAccount,
        TransactionDocumentItem item,
        PayerPayeeId payerPayeeId,
        SubCategoryId subCategoryId,
        IEnumerable<string>? tags,
        string title,
        DateTime transactionDate,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) :
        base(TransactionEnums.TransactionKeys.PAYMENT_OUT)
    {
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Description = description;
        ExtTransactionNo = extTransactionNo;
        IsPaidToOwnAccount = isPaidToOwnAccount;
        Items = [item];
        PayerPayeeId = payerPayeeId;
        SubCategoryId = subCategoryId;
        Tags = tags?.ToList() ?? [];
        Title = title;
        TransactionDate = transactionDate;
        TransactionType = TransactionEnums.TransactionKeys.PAYMENT_OUT;

        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;
    }
}

/// <summary>
/// For when the Account purchases something from a "Merchant"
/// </summary>
public sealed class PurchasesTransactionDocument() :
    DebitTransactionDocument(TransactionEnums.TransactionKeys.PURCHASE)
{
    [BsonElement("total_owing")]
    public decimal TotalOwing => TotalAmount - TotalPaid;

    [BsonElement("is_paid")]
    public bool IsPaid => PaidDate.HasValue;

    [BsonElement("payments")]
    public List<TransactionDocumentPayment> Payments { get; set; } = [];

    [BsonElement("total_paid")]
    public decimal TotalPaid => Payments.Sum(p => p.Amount);

    [BsonElement("paid_date")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime? PaidDate { get; set; } = null;
}


public sealed record TransactionDocumentItem
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }

    [BsonElement("item_amount")]
    public decimal Amount { get; set; }

    [BsonElement("category_id")]
    public ObjectId? CategoryId { get; set; }

    [BsonElement("sub_category_id")]
    public ObjectId? SubCategoryId { get; set; }

    [BsonElement("description")]
    public string Description { get; init; }

    [BsonElement("is_paid")]
    public bool IsPaid => PaidDate.HasValue;

    [BsonElement("paid_date")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime? PaidDate { get; set; }

    public TransactionDocumentItem(
        ObjectId id,
        decimal amount,
        ObjectId? categoryId,
        ObjectId? subCategoryId,
        string description)
    {
        Id = id;
        Amount = amount;
        Description = description;
        CategoryId = categoryId;
        SubCategoryId = subCategoryId;
    }

    public static TransactionDocumentItem New(
        decimal amount,
        string description,
        string categoryId = "",
        string subCategoryId = "")
    {
        ObjectId? categoryObjectId = !string.IsNullOrEmpty(categoryId) ? ObjectId.Parse(categoryId) : null;
        ObjectId? subCategoryObjectId = !string.IsNullOrEmpty(subCategoryId) ? ObjectId.Parse(subCategoryId) : null;

        return new TransactionDocumentItem(ObjectId.GenerateNewId(), amount, categoryObjectId, subCategoryObjectId, description);
    }
}


public sealed record TransactionDocumentPayment(ObjectId Id, decimal Amount, DateTime PaymentDate)
{
    [BsonElement("id")]
    public ObjectId Id { get; set; } = Id;

    public static TransactionDocumentPayment New(decimal amount, DateTime paymentDate)
    {
        return new TransactionDocumentPayment(ObjectId.GenerateNewId(), amount, paymentDate);
    }
}