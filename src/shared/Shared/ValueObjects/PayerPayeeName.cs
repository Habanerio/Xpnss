namespace Habanerio.Xpnss.Shared.ValueObjects;

public readonly record struct PayerPayeeName
{
    public const int MAX_LENGTH = 50;

    public readonly string Value;

    public PayerPayeeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));// InvalidAccountNameException();

        if (name.Length > MAX_LENGTH)
            Value = name[..MAX_LENGTH];

        Value = name;
    }

    public static implicit operator string(PayerPayeeName name) => name.Value;

    //public static implicit operator Name(string name) => new(name);
}