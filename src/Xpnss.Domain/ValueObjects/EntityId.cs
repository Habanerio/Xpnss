using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.ValueObjects;

public abstract record EntityId
{
    private string _value;

    public string Value => _value;

    protected EntityId()
    {
        _value = string.Empty;
    }

    protected EntityId(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException(nameof(entityId));// InvalidAccountNameException();

        if (entityId.Length > 50)
            throw new ArgumentOutOfRangeException(nameof(entityId));// AccountNameTooLongException(accountName);

        _value = entityId;
    }

    internal void SetValue(string value)
    {
        _value = value;
    }

    public static implicit operator string(EntityId entityId) => entityId.Value;

    //public static implicit operator AccountId(string userId) => new(userId);
}

public record EntityObjectId : EntityId
{
    protected EntityObjectId() : base(ObjectId.GenerateNewId().ToString())
    { }

    protected EntityObjectId(ObjectId entityId) : base(entityId.ToString())
    {
        if (entityId.Equals(ObjectId.Empty))
            throw new ArgumentException(nameof(entityId));
    }

    protected EntityObjectId(string entityId)
    {
        if (!ObjectId.TryParse(entityId, out var objectId) || objectId.Equals(ObjectId.Empty))
            throw new ArgumentException(nameof(entityId));

        SetValue(entityId);
    }
}