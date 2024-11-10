using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public sealed record MerchantId : EntityObjectId
{
    private MerchantId()
    {
        SetValue(ObjectId.Empty.ToString());
    }

    public MerchantId(string? merchantId)
    {
        SetValue(merchantId ?? string.Empty);
    }

    public MerchantId(ObjectId merchantId)
    {
        SetValue(merchantId.ToString());
    }

    public static MerchantId New => new MerchantId(ObjectId.GenerateNewId().ToString());

    public static MerchantId Empty => new MerchantId();

    public static implicit operator string(MerchantId merchantId) => merchantId.Value;

    ////public static implicit operator AccountId(string userId) => new(userId);
}