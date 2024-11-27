namespace Habanerio.Xpnss.Domain.Types;

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

    public static IReadOnlyCollection<Keys> CreditAccountTypes =>
    new[]
    {
        Keys.CreditCard,
        Keys.LineOfCredit,
        Keys.Loan,
        Keys.Mortgage,
    };

    public static IReadOnlyCollection<Keys> DebitAccountTypes =>
    new[]
    {
        Keys.Cash,
        Keys.Checking,
        Keys.Savings,
        Keys.Investment,
    };

    public static bool IsCreditAccount(Keys accountType) =>
        CreditAccountTypes.Contains(accountType);
}