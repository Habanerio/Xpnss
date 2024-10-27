namespace Habanerio.Xpnss.Modules.Transactions.Common;

public class TransactionTypes
{
    public enum Keys
    {
        CHARGE,
        CREDIT,
        DEPOSIT,
        FEE,
        INCOME,
        INTEREST_CHARGE,
        INTEREST_EARNED,
        PURCHASE,
        REFUND,
        TRANSFER,
        WITHDRAWAL,
    }

    public static IReadOnlyCollection<Keys> CreditTransactionTypes => new[]
    {
        Keys.CREDIT,
        Keys.DEPOSIT,
        Keys.INCOME,
        Keys.INTEREST_EARNED,
        Keys.REFUND,
    };

    public static IReadOnlyCollection<Keys> DebitTransactionTypes => new[]
    {
        Keys.CHARGE,
        Keys.FEE,
        Keys.INTEREST_CHARGE,
        Keys.PURCHASE,
        Keys.TRANSFER,
        Keys.WITHDRAWAL,
    };

    public static IReadOnlyCollection<Keys> AllTransactionTypes =>
        Enum.GetValues<Keys>();

    public static IReadOnlyCollection<string> AllTransactionTypeStrings()
    {
        var allTransactionTypes = Enum.GetValues<Keys>();

        return allTransactionTypes.Select(t => t.ToString()).ToList();
    }


    public static bool IsCredit(Keys transactionType) =>
        CreditTransactionTypes.Contains(transactionType);

    public static bool IsCredit(string transactionType) =>
        CreditTransactionTypes.Any(t => t.ToString().Equals(transactionType));

    public static bool IsDebit(Keys transactionType) =>
        DebitTransactionTypes.Contains(transactionType);

    public static bool IsDebit(string transactionType) =>
        DebitTransactionTypes.Any(t => t.ToString().Equals(transactionType));

    public static TransactionTypes.Keys ToTransactionType(string transactionType) =>
        Enum.Parse<TransactionTypes.Keys>(transactionType);
}