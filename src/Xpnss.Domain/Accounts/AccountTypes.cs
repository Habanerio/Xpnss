namespace Habanerio.Xpnss.Domain.Accounts;

public static class AccountTypes
{
    public enum Keys
    {
        None = 0,
        Cash,
        Checking,
        Savings,
        Investment,
        CreditCard,
        LineOfCredit,
        Loan,
        Mortgage,
    }

    public static IReadOnlyCollection<Keys> AllAccountTypes =>
        Enum.GetValues<Keys>();

    public static IReadOnlyCollection<string> AllAccountTypeStrings()
    {
        var allAccountTypes = Enum.GetValues<Keys>();

        return allAccountTypes.Select(t => t.ToString()).ToList();
    }
}