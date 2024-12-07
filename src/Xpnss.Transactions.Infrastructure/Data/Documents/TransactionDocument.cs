using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Domain.Types;
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


    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("ext_transaction_id")]
    public string ExtTransactionId { get; set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }

    [BsonElement("is_paid")]
    public bool IsPaid => PaidDate.HasValue;

    [BsonElement("items")]
    public List<TransactionDocumentItem> Items { get; set; }

    [BsonElement("payerpayee_id")]
    public ObjectId? PayerPayeeId { get; set; }

    [BsonElement("payments")]
    public List<TransactionDocumentPayment> Payments { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; }

    [BsonElement("total_amount")]
    public decimal TotalAmount { get; set; }

    [BsonElement("total_owing")]
    public decimal TotalOwing { get; set; }

    [BsonElement("total_paid")]
    public decimal TotalPaid { get; set; }

    [BsonElement("transaction_date")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime TransactionDate { get; set; }

    [BsonElement("paid_date")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime? PaidDate { get; set; }


    [BsonElement("date_created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateCreated { get; set; }

    [BsonElement("date_updated")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateUpdated { get; set; }

    [BsonElement("date_deleted")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateDeleted { get; set; }

    public TransactionDocument()
    {
        Id = ObjectId.GenerateNewId();
        Items = [];
        Payments = [];
    }
}

public class DepositTransactionDocument : TransactionDocument
{
    public DepositTransactionDocument()
    {
        TransactionType = TransactionEnums.TransactionKeys.DEPOSIT;
    }
}

/// <summary>
/// For when the Account purchases something from a "Merchant"
/// </summary>
public class PurchaseTransactionDocument : TransactionDocument
{
    public PurchaseTransactionDocument()
    {
        TransactionType = TransactionEnums.TransactionKeys.PURCHASE;
    }
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

public sealed record TransactionDocumentPayment
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }

    public decimal Amount { get; init; }

    public DateTime PaymentDate { get; init; }

    public TransactionDocumentPayment(ObjectId id, decimal amount, DateTime paymentDate)
    {
        Id = id;
        Amount = amount;
        PaymentDate = paymentDate;
    }

    public static TransactionDocumentPayment New(decimal amount, DateTime paymentDate)
    {
        return new TransactionDocumentPayment(ObjectId.GenerateNewId(), amount, paymentDate);
    }
}