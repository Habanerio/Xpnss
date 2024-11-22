namespace Habanerio.Xpnss.Domain.Types;

public static class TransactionTypes
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
        PAYMENT,
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
        Keys.PAYMENT,
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


    public static bool IsCreditTransaction(Keys transactionType) =>
          CreditTransactionTypes.Contains(transactionType);

    public static bool IsCreditTransaction(string transactionType) =>
        CreditTransactionTypes.Any(t => t.ToString().Equals(transactionType));


    public static bool IsCreditTransaction(AccountTypes.Keys accountType, Keys transactionType) =>

        AccountTypes.IsCreditAccount(accountType) && CreditTransactionTypes.Contains(transactionType) ||

        !AccountTypes.IsCreditAccount(accountType) && CreditTransactionTypes.Contains(transactionType);

    public static Keys ToTransactionType(string transactionType) =>
        Enum.Parse<Keys>(transactionType);
}