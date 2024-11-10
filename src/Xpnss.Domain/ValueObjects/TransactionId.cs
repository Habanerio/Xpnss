using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public record TransactionId : EntityObjectId
{
    public TransactionId(string transactionId) : base(transactionId)
    { }

    public TransactionId(ObjectId transactionId) : base(transactionId)
    { }

    public static TransactionId New => new TransactionId(ObjectId.GenerateNewId().ToString());

    public static implicit operator string(TransactionId transactionId) => transactionId.Value;
}

public record TransactionItemId : EntityObjectId
{
    public TransactionItemId(string transactionId) : base(transactionId)
    { }

    public TransactionItemId(ObjectId transactionId) : base(transactionId)
    { }

    public static TransactionItemId New => new TransactionItemId(ObjectId.GenerateNewId().ToString());

    public static implicit operator string(TransactionItemId transactionItemId) => transactionItemId.Value;
}

public record TransactionPaymentId : EntityObjectId
{
    public TransactionPaymentId(string transactionId) : base(transactionId)
    { }

    public TransactionPaymentId(ObjectId transactionId) : base(transactionId)
    { }

    public static TransactionPaymentId New => new TransactionPaymentId(ObjectId.GenerateNewId().ToString());

    public static implicit operator string(TransactionPaymentId transactionPaymentId) => transactionPaymentId.Value;
}