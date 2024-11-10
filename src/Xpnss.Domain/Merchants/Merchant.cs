using Habanerio.Xpnss.Domain.Accounts;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Merchants;

public sealed class Merchant : AggregateRoot<MerchantId>
{
    private readonly List<MonthlyMerchantTotals> _monthlyTotals = [];

    public UserId UserId { get; set; }

    public MerchantName Name { get; set; }

    public string Location { get; set; }

    public IReadOnlyCollection<MonthlyMerchantTotals> MonthlyTotals => _monthlyTotals.AsReadOnly();

    private Merchant(UserId userId, MerchantName name, string location) : this(MerchantId.New, userId, name, location)
    {
        IsTransient = true;
    }

    private Merchant(MerchantId id, UserId userId, MerchantName name, string location) : base(id)
    {
        UserId = userId;
        Name = name;
        Location = location;

        _monthlyTotals = [];
    }

    public static Merchant Load(MerchantId id, UserId userId, MerchantName name, string location = "")
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException($"{nameof(id)} cannot be null or whitespace.", nameof(id));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException($"{nameof(userId)} cannot be null or whitespace.", nameof(userId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));

        return new Merchant(id, userId, name, location);
    }

    public static Merchant New(UserId userId, MerchantName name, string location)
    {
        return new Merchant(userId, name, location);
    }
}