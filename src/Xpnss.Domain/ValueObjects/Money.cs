namespace Habanerio.Xpnss.Domain.ValueObjects;

public readonly record struct Money(decimal Value)
{

    public static Money Zero => new(0);

    public static Money operator +(Money a, Money b) => new(a.Value + b.Value);

    public static Money operator -(Money a, Money b) => new(a.Value - b.Value);

    public static bool operator >(Money a, Money b) => a.Value > b.Value;

    public static bool operator >=(Money a, Money b) => a.Value >= b.Value;

    public static bool operator <(Money a, Money b) => a.Value < b.Value;

    public static bool operator <=(Money a, Money b) => a.Value <= b.Value;

    public static implicit operator decimal(Money money) => money.Value;
}