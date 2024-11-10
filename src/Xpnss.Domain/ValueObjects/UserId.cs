namespace Habanerio.Xpnss.Domain.ValueObjects;

public record UserId : EntityId
{
    public UserId(string userId) : base(userId)
    { }

    public static implicit operator string(UserId userId) => userId.Value;

    //public static implicit operator UserId(string userId) => new(userId);
}