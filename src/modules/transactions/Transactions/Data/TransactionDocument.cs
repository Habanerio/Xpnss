using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Modules.Transactions.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Modules.Transactions.Data;

[BsonCollection("money_transactions")]
public class TransactionDocument : MongoDocument
{
    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("account_id")]
    public ObjectId AccountId { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("merchant")]
    public TransactionMerchant? Merchant { get; set; }

    [BsonElement("items")]
    public List<TransactionItem> Items { get; set; } = [];

    [BsonElement("payments")]
    public List<TransactionPayment> Payments { get; set; } = [];

    [BsonElement("total_amount")]
    public decimal TotalAmount { get; set; }

    [BsonElement("total_owing")]
    public decimal TotalOwing { get; set; }

    [BsonElement("total_paid")]
    public decimal TotalPaid { get; set; }

    [BsonElement("transaction_date")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime TransactionDate { get; set; }

    [BsonElement("transaction_type")]
    public TransactionTypes.Keys TransactionType { get; set; }

    [BsonElement("date_paid")]
    [BsonDateTimeOptions(DateOnly = true)]
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
        string accountId,
        DateTime transactionDate,
        List<TransactionItem> transactionItems,
        TransactionTypes.Keys transactionTypes,
        string description = "",
        TransactionMerchant? merchant = null
    )
    {
        var transaction = new TransactionDocument
        {
            UserId = userId,
            AccountId = ObjectId.Parse(accountId),
            Items = transactionItems,
            Merchant = merchant,
            TransactionDate = transactionDate.ToUniversalTime().Date,
            TransactionType = transactionTypes,
            Description = description,
            DateCreated = DateTime.UtcNow
        };

        transaction.TotalAmount = transactionItems.Sum(x => x.Amount);
        transaction.TotalOwing = transaction.TotalAmount;
        transaction.TotalPaid = 0;

        return transaction;
    }

    public decimal AddPayment(string userId, decimal amount, DateTime paymentDate)
    {
        var paymentToApply = amount > TotalOwing ? TotalOwing : amount;
        var remaining = amount > TotalOwing ? amount - TotalOwing : 0;

        ApplyPayment(paymentToApply, paymentDate);

        return remaining;
    }

    private void ApplyPayment(decimal amount, DateTime paymentDate)
    {
        TotalPaid += amount;
        Payments.Add(TransactionPayment.New(amount, paymentDate));

        DateUpdated = DateTime.UtcNow;

        if (TotalOwing <= 0)
        {
            DatePaid = paymentDate.ToUniversalTime();
        }
    }

    public void AddItem(decimal amount, string description, string categoryId = "")
    {
        Items.Add(TransactionItem.New(amount, description, categoryId));

        TotalAmount += amount;
        TotalOwing = TotalAmount - TotalPaid;
    }
}

public sealed record TransactionItem
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }

    [BsonElement("item_amount")]
    public decimal Amount { get; set; }

    [BsonElement("category_id")]
    public ObjectId? CategoryId { get; set; }

    [BsonElement("description")]
    public string Description { get; init; }

    public TransactionItem(ObjectId id, decimal amount, string description, ObjectId categoryId)
    {
        Id = id;
        Amount = amount;
        Description = description;
        CategoryId = categoryId;
    }

    public static TransactionItem New(decimal amount, string description, string categoryId = "")
    {
        ObjectId categoryObjectId = !string.IsNullOrEmpty(categoryId) ? ObjectId.Parse(categoryId) : ObjectId.Empty;

        return new TransactionItem(ObjectId.GenerateNewId(), amount, description, categoryObjectId);
    }
}

public sealed record TransactionMerchant
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("locations")]
    public string Location { get; set; }

    public TransactionMerchant(string id, string name, string location)
    {
        Id = ObjectId.Parse(id);
        Name = name;
        Location = location;
    }

    public static TransactionMerchant New(string name, string location)
    {
        return new TransactionMerchant(ObjectId.GenerateNewId().ToString(), name, location);
    }
}

public sealed record TransactionPayment
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }

    public decimal Amount { get; init; }

    public DateTimeOffset PaymentDate { get; init; }

    public TransactionPayment(ObjectId id, decimal amount, DateTimeOffset paymentDate)
    {
        Id = id;
        Amount = amount;
        PaymentDate = paymentDate;
    }

    public static TransactionPayment New(decimal amount, DateTimeOffset paymentDate)
    {
        return new TransactionPayment(ObjectId.GenerateNewId(), amount, paymentDate);
    }
}