namespace Habanerio.Xpnss.Domain.ValueObjects;

public readonly record struct AccountName
{
    public const int MAX_LENGTH = 50;

    public readonly string Value;

    public AccountName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));// InvalidAccountNameException();

        if (name.Length > MAX_LENGTH)
            Value = name[..MAX_LENGTH];

        Value = name;
    }

    public static implicit operator string(AccountName name) => name.Value;

    //public static implicit operator Name(string name) => new(name);
}