namespace Habanerio.Xpnss.Domain.ValueObjects;

public readonly record struct MerchantName
{
    public const int MAX_LENGTH = 32;

    public readonly string Value;

    public MerchantName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));// InvalidAccountNameException();

        if (name.Length > MAX_LENGTH)
            Value = name[..MAX_LENGTH];

        Value = name;
    }

    public static implicit operator string(MerchantName name) => name.Value;

    //public static implicit operator Name(string name) => new(name);
}