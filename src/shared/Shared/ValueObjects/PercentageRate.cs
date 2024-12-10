namespace Habanerio.Xpnss.Shared.ValueObjects;

public readonly record struct PercentageRate
{
    public decimal Value { get; }

    public PercentageRate(decimal percentageRate)
    {
        if (percentageRate is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(percentageRate), "Percentage rate must be between 0 and 100.");

        Value = percentageRate;
    }

    public static Money operator +(PercentageRate a, PercentageRate b) => new(a.Value + b.Value);

    public static Money operator -(PercentageRate a, PercentageRate b) => new(a.Value - b.Value);

    public static bool operator >=(PercentageRate a, PercentageRate b) => a.Value >= b.Value;

    public static bool operator <=(PercentageRate a, PercentageRate b) => a.Value <= b.Value;

    public static implicit operator decimal(PercentageRate rate) => rate.Value;

    // Leaving commented out for now. Want to force the use of new PercentageRate(decimal) constructor.
    //public static implicit operator PercentageRate(decimal value) => new(value);

    public override string ToString() => $"{Value}%";

    public decimal Calculate(decimal amount) => amount * Value / 100;
}