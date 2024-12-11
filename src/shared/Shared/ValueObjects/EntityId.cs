using MongoDB.Bson;

namespace Habanerio.Xpnss.Shared.ValueObjects;

public abstract record EntityId
{
    private string? _value;

    public virtual string Value => _value ?? string.Empty;

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

    internal void SetValue(string? value)
    {
        _value = value;
    }

    public static implicit operator string(EntityId entityId) => entityId.Value;

    //public static implicit operator AccountId(string userId) => new(userId);
}

public record EntityObjectId : EntityId
{
    public EntityObjectId() : base(ObjectId.GenerateNewId().ToString())
    { }

    public EntityObjectId(ObjectId entityId) : base(entityId.ToString())
    {
        //if (entityId.Equals(ObjectId.Empty))
        //    throw new ArgumentException(nameof(entityId));
    }

    public EntityObjectId(string entityId)
    {
        if (!ObjectId.TryParse(entityId, out var objectId) || objectId.Equals(ObjectId.Empty))
            throw new ArgumentException(nameof(entityId));

        SetValue(entityId);
    }

    public static EntityObjectId New => new EntityObjectId(ObjectId.GenerateNewId().ToString());

    public static EntityObjectId Empty => new EntityObjectId(ObjectId.Empty);

    public static bool IsEmpty(EntityObjectId entityId) => entityId == Empty;

    public static implicit operator string(EntityObjectId entityId) => entityId.Value;

    /// <summary>
    /// For non-nullable ObjectId, return the value if it can be parsed, otherwise return ObjectId.Empty
    /// </summary>
    /// <param name="entityId"></param>
    public static implicit operator ObjectId(EntityObjectId entityId) =>
           ObjectId.TryParse(entityId.Value, out var objectId) ? objectId : Empty;

    ///// <summary>
    ///// For nullable ObjectId, return the value if it can be parsed, otherwise return null
    ///// </summary>
    ///// <param name="entityId"></param>
    //public static implicit operator ObjectId?(EntityObjectId entityId) =>
    //    ObjectId.TryParse(entityId.Value, out var objectId) ?
    //        objectId :
    //        null;

    //public static implicit operator AccountId(string userId) => new(userId);
}