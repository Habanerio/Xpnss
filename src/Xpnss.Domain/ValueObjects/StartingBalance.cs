namespace Habanerio.Xpnss.Domain.ValueObjects;

public readonly record struct StartingBalance
{
    public Money Balance { get; }

    public DateTime Date { get; }

    public StartingBalance(decimal balance, DateTime date)
    {
        Balance = new Money(balance);
        Date = date;
    }
}