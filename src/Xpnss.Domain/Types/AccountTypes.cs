namespace Habanerio.Xpnss.Domain.Types;

public static class AccountTypes
{
    public enum Keys
    {
        NONE = 0,

        CASH,
        CHECKING,
        SAVINGS,
        INVESTMENT,

        CREDIT_CARD,
        LINE_OF_CREDIT,
        LOAN,
        MORTGAGE,

        UNKNOWN = 999
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
        Keys.CREDIT_CARD,
        Keys.LINE_OF_CREDIT,
        Keys.LOAN,
        Keys.MORTGAGE,
    };

    public static IReadOnlyCollection<Keys> DebitAccountTypes =>
    new[]
    {
        Keys.CASH,
        Keys.CHECKING,
        Keys.SAVINGS,
        Keys.INVESTMENT,
    };

    public static bool IsCreditAccount(Keys accountType) =>
        CreditAccountTypes.Contains(accountType);
}