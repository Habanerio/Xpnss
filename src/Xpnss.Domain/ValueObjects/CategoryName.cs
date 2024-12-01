namespace Habanerio.Xpnss.Domain.ValueObjects;

public readonly record struct CategoryName
{
    public const int MAX_LENGTH = 50;

    public readonly string Value;

    public CategoryName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));// InvalidAccountNameException();

        if (name.Length > MAX_LENGTH)
            Value = name[..MAX_LENGTH];

        Value = name;
    }

    public bool Equals(CategoryName? other)
    {
        if (other is null)
            return false;

        return Value.Equals(other.Value);
    }

    public static implicit operator string(CategoryName name) => name.Value;

    //public static implicit operator Name(string name) => new(name);
}