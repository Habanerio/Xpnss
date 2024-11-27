using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

/// <summary>
/// Represents a Transaction's Id
/// </summary>
public record TransactionId : EntityObjectId
{
    public TransactionId(string transactionId) : base(transactionId) { }

    public TransactionId(ObjectId transactionId) : base(transactionId) { }


    public new static TransactionId New => new(ObjectId.GenerateNewId().ToString());

    public new static TransactionId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(TransactionId transactionId) => transactionId.Equals(Empty);


    public static implicit operator string(TransactionId transactionId) => transactionId.Value;
}


/// <summary>
/// Represents a Transaction Item's Id
/// </summary>
public record TransactionItemId : EntityObjectId
{
    public TransactionItemId(string transactionItemId) : base(transactionItemId) { }

    public TransactionItemId(ObjectId transactionItemId) : base(transactionItemId) { }


    public new static TransactionItemId New => new(ObjectId.GenerateNewId().ToString());

    public new static TransactionItemId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(TransactionItemId transactionItemId) => transactionItemId.Equals(Empty);


    public static implicit operator string(TransactionItemId transactionItemId) => transactionItemId.Value;
}


/// <summary>
/// Represents a Transaction Payment's Id
/// </summary>
public record TransactionPaymentId : EntityObjectId
{
    public TransactionPaymentId(string transactionPaymentId) : base(transactionPaymentId) { }

    public TransactionPaymentId(ObjectId transactionPaymentId) : base(transactionPaymentId) { }


    public new static TransactionPaymentId New => new(ObjectId.GenerateNewId().ToString());

    public new static TransactionPaymentId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(TransactionPaymentId transactionPaymentId) => transactionPaymentId.Equals(Empty);


    public static implicit operator string(TransactionPaymentId transactionPaymentId) => transactionPaymentId.Value;
}