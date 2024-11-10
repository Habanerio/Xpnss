using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Domain.Transactions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Infrastructure.Documents;

[BsonCollection("money_accounts_transactions")]
public class TransactionDocument : MongoDocument
{
    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("account_id")]
    public ObjectId AccountId { get; set; }

    [BsonElement("transaction_type")]
    [BsonRepresentation(BsonType.String)]
    public TransactionTypes.Keys TransactionType { get; set; }


    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("merchant_id")]
    public string? MerchantId { get; set; }

    [BsonElement("items")]
    public List<TransactionDocumentItem> Items { get; set; }

    [BsonElement("payments")]
    public List<TransactionDocumentPayment> Payments { get; set; }

    [BsonElement("total_amount")]
    public decimal TotalAmount { get; set; }

    [BsonElement("total_owing")]
    public decimal TotalOwing { get; set; }

    [BsonElement("total_paid")]
    public decimal TotalPaid { get; set; }

    [BsonElement("transaction_date")]
    [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Utc)]
    public DateTime TransactionDate { get; set; }

    [BsonElement("date_paid")]
    [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Utc)]
    public DateTime? DatePaid { get; set; }

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

    public static TransactionDocument New(
        string userId,
        ObjectId accountId,
        TransactionTypes.Keys transactionType,
        string description,
        ObjectId? merchantId,
        List<TransactionDocumentItem> items,
        DateTime dateOfTransaction)
    {
        return new TransactionDocument()
        {
            UserId = userId,
            AccountId = accountId,
            Description = description,
            MerchantId = merchantId?.ToString() ?? null,
            TransactionDate = dateOfTransaction.ToUniversalTime().Date,
            TransactionType = transactionType,

            TotalAmount = items.Sum(i => i.Amount),
            TotalOwing = items.Sum(i => i.Amount),
            TotalPaid = 0,

            DateCreated = DateTime.UtcNow
        };
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

    [BsonElement("description")]
    public string Description { get; init; }

    public TransactionDocumentItem(ObjectId id, decimal amount, ObjectId? categoryId, string description)
    {
        Id = id;
        Amount = amount;
        Description = description;
        CategoryId = categoryId;
    }

    public static TransactionDocumentItem New(decimal amount, string description, string categoryId = "")
    {
        ObjectId? categoryObjectId = !string.IsNullOrEmpty(categoryId) ? ObjectId.Parse(categoryId) : null;

        return new TransactionDocumentItem(ObjectId.GenerateNewId(), amount, categoryObjectId, description);
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