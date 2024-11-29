using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public sealed record PayerPayeeId : EntityObjectId
{
    private PayerPayeeId() : this(ObjectId.Empty.ToString())
    { }

    public PayerPayeeId(ObjectId? id) : this(id?.ToString())
    { }

    public PayerPayeeId(string? id)
    {
        SetValue(id ?? string.Empty);
    }

    public new static PayerPayeeId New => new(ObjectId.GenerateNewId());

    public new static PayerPayeeId Empty => new(ObjectId.Empty);

    public static bool IsEmpty(PayerPayeeId id) => id.Equals(Empty);


    public static implicit operator string(PayerPayeeId id) => id.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}

//public sealed record PayerPayeeId : EntityObjectId
//{
//    private PayerPayeeId()
//    {
//        SetValue(ObjectId.Empty.ToString());
//    }

//    public PayerPayeeId(string? id)
//    {
//        SetValue(id);
//    }

//    public PayerPayeeId(ObjectId? id)
//    {
//        SetValue(id.ToString());
//    }

//    //internal void SetNullValue(string? value)
//    //{
//    //    base.SetValue(value);
//    //}

//    public new static PayerPayeeId New => new(ObjectId.GenerateNewId());

//    //public new static PayerPayeeId Null
//    //{
//    //    get
//    //    {
//    //        var payerPayeeId = new PayerPayeeId();
//    //        payerPayeeId.SetNullValue(null);

//    //        return payerPayeeId;
//    //    }
//    //}

//    public new static PayerPayeeId Empty => new();

//    public static bool IsEmpty(PayerPayeeId id) => id.Equals(Empty);


//    public static implicit operator string(PayerPayeeId? id) => id?.Value ?? string.Empty;

//    ////public static implicit operator AccountId(string userId) => new(userId);
//}