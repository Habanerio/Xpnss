using System.ComponentModel.DataAnnotations;

namespace Habanerio.Xpnss.Domain.Types;

public static class TransactionEnums
{
    public enum TransactionKeys
    {
        [Display(Name = "Adjustment", Description = "Adjustment to increase or decrease the balance due to a correction by the institution")]
        ADJUSTMENT,

        /// <summary>
        /// Adjustments to increase the balance (e.g., bank error correction).
        /// </summary>
        [Display(Name = "Adjustment Credit", Description = "Adjustments to increase the balance (e.g., bank error correction).")]
        ADJUSTMENT_CREDIT,

        /// <summary>
        /// Adjustments to decrease the balance (e.g., bank error correction).
        /// </summary>
        [Display(Name = "Adjustment Debit", Description = "Adjustments to decrease the balance (e.g., bank error correction).")]
        ADJUSTMENT_DEBIT,


        [Display(Name = "Balance Transfer", Description = "Specific type of transfer between two accounts. Usually from a Credit Card to some other account")]
        BALANCE_TRANSFER,

        // Not sure if this is needed? It's just a Deposit?
        [Display(Name = "Balance Transfer In", Description = "Money transferred into the account from another account.")]
        BALANCE_TRANSFER_IN,

        [Display(Name = "Balance Transfer Out", Description = "Money transferred out of the account to another account.")]
        BALANCE_TRANSFER_OUT,


        ///// <summary>
        ///// Bonuses credited to the account (e.g., for opening an account).
        ///// </summary>
        //[Display(Name = "Bonus", Description = "Bonuses credited to the account (e.g., for opening an account).")]
        //BONUS,

        // Just a withdrawal?
        /// <summary>
        /// Borrowing money against a credit card or line of credit.
        /// </summary>
        [Display(Description = "Borrowing money against a credit card or line of credit.")]
        CASH_ADVANCE,

        //[Obsolete("Same as Deposit?")]
        //[Display(Name = "Credit", Description = "")]
        //CREDIT,

        [Display(Name = "Deposit", Description = "Adding money to the account (e.g., paycheck, cash deposit).")]
        DEPOSIT,

        [Display(Name = "Dividend Received", Description = "Dividend payments credited to the account.")]
        DIVIDEND,

        /// <summary>
        /// General term for charges applied to the account (e.g., late fees).
        /// </summary>
        [Display(Name = "Fee", Description = "General term for charges applied to the account (e.g., late fees).")]
        CHARGE_FEE,

        // Can this be a Deposit on a Cash Account, and assigned to an "Income" Category?
        //[Display(Name = "Income", Description = "Money earned from doing work (or selling something)")]
        //INCOME,

        [Display(Name = "Interest Charge", Description = "Interest charges to an account such as a Credit Card, Line of Credit, Loan, ...")]
        INTEREST_CHARGE,

        // Can this be a Deposit on a Savings or Investment Account, and assigned to a "Interest Earned" Category?
        [Display(Description = "Interest credited to the account (e.g., savings or checking account).")]
        INTEREST_EARNED,

        [Display(Name = "Other", Description = "For all other types of transactions.")]
        OTHER,

        [Display(Description = "")]
        PAYMENT,

        [Display(Name = "Payment In", Description = "A payment made to an account")]
        PAYMENT_IN,

        [Display(Name = "Payment Out", Description = "A payment made from an account")]
        PAYMENT_OUT,

        [Display(Description = "Spending money for goods or services.")]
        PURCHASE,

        [Display(Name = "Refund / Reimbursement", Description = "Money received from a prior purchase or from money lent")]
        REFUND_REIMBURSEMENT,


        [Display(Name = "Transfer", Description = "The action the user would take to Transfer Out of one account and Transfer In to another account")]
        TRANSFER,

        // This would be the actual Transfer Transaction on the account that is receiving
        [Display(Name = "Transfer In", Description = "Money transferred into the account from another account.")]
        TRANSFER_IN,

        // This would be the actual Transfer Transaction on the account that is paying
        [Display(Name = "Transfer Out", Description = "Money transferred out of the account to another account.")]
        TRANSFER_OUT,

        [Display(Description = "Taking money out of the account (e.g., ATM or cash withdrawal).")]
        WITHDRAWAL,
    }

    public static Dictionary<int, string> ToDictionary()
    {
        return Enum.GetValues<TransactionKeys>()
            .ToDictionary(k => (int)k, v => v.ToString());
    }

    public static IReadOnlyCollection<TransactionKeys> CreditTransactionKeys => new[]
    {
        TransactionKeys.ADJUSTMENT_CREDIT,
        TransactionKeys.BALANCE_TRANSFER_IN,
        TransactionKeys.DEPOSIT,
        TransactionKeys.DIVIDEND,
        TransactionKeys.INTEREST_EARNED,
        TransactionKeys.PAYMENT_IN,
        TransactionKeys.REFUND_REIMBURSEMENT,
        TransactionKeys.TRANSFER_IN,
    };

    public static IReadOnlyCollection<TransactionKeys> DebitTransactionKeys => new[]
    {
        TransactionKeys.ADJUSTMENT_DEBIT,
        TransactionKeys.BALANCE_TRANSFER_OUT,
        TransactionKeys.CHARGE_FEE,
        TransactionKeys.INTEREST_CHARGE,
        TransactionKeys.PAYMENT_OUT,
        TransactionKeys.PURCHASE,
        TransactionKeys.TRANSFER_OUT,
        TransactionKeys.WITHDRAWAL,
    };

    public static IReadOnlyCollection<TransactionKeys> AllTransactionTypes =>
        Enum.GetValues<TransactionKeys>();

    public static IReadOnlyCollection<string> AllTransactionTypeStrings() =>
        Enum.GetValues<TransactionKeys>().Select(t => t.ToString()).ToList();

    public static IReadOnlyCollection<TransactionKeys> UserTransactionTypes() =>
    [
        TransactionKeys.BALANCE_TRANSFER,
        //  // Would this just be a "Deposit"?
        //  - AccountEnums.CurrencyKeys.BALANCE_TRANSFER_IN,
        //  - AccountEnums.CurrencyKeys.BALANCE_TRANSFER_OUT,
        // CASH_ADVANCE: If the Credit Account has business logic for this, may be useful?
        // AccountEnums.CurrencyKeys.CASH_ADVANCE,
        TransactionKeys.CHARGE_FEE,
        TransactionKeys.DEPOSIT,
        // DIVIDEND: IF there's an Investment Account, then we may need this,
        // instead of putting it under "Deposit" (or "Income") and applying a "Dividend" category?
        TransactionKeys.DIVIDEND,
        // INCOME: Can this be a DEPOSIT and fall under an "INCOME" Category?
        //AccountEnums.CurrencyKeys.INCOME,
        // INTEREST_CHARGE: Can this be a SERVICE_FEE and fall under an "INTEREST" Category?
        // AccountEnums.CurrencyKeys.INTEREST_CHARGE,
        // INTEREST_EARNED: Can this be a DEPOSIT and fall under an "INTEREST" Category? If there's an Investment Account, then we may need this,
        TransactionKeys.INTEREST_EARNED,
        TransactionKeys.PAYMENT, 
        //  - AccountEnums.CurrencyKeys.PAYMENT_IN,
        //  - AccountEnums.CurrencyKeys.PAYMENT_OUT,
        TransactionKeys.PURCHASE,
        TransactionKeys.REFUND_REIMBURSEMENT,
        // Is there a difference between the following two? Or group them together?
        //  - AccountEnums.CurrencyKeys.REFUND?,
        //  - AccountEnums.CurrencyKeys.REIMBURSEMENT?,
        TransactionKeys.TRANSFER,
        //  - AccountEnums.CurrencyKeys.TRANSFER_IN,
        //  - AccountEnums.CurrencyKeys.TRANSFER_OUT,
        TransactionKeys.WITHDRAWAL
    ];

    public static bool IsCreditTransaction(TransactionKeys transactionType) =>
          CreditTransactionKeys.Contains(transactionType);

    public static bool IsCreditTransaction(string transactionType) =>
        CreditTransactionKeys.Any(t => t.ToString().Equals(transactionType));

    /// <summary>
    /// Determines whether the account should be credited or debited based on the account type and transaction type.
    /// Credit Account: Balance increases with Credits (purchases) and decreases with Debits (deposits).
    /// Debit Account: Balance increases with Debits (deposits) and decreases with Credits (purchases).
    /// </summary>
    /// <returns></returns>
    public static bool DoesBalanceIncrease(bool isCreditAccount, TransactionKeys transactionType)
    {
        // DEPOSIT + CHECKING = Balance Increases (GOOD Thing)
        // PURCHASE + CREDIT CARD = Balance (owed) Increases (BAD Thing)
        var check1 = isCreditAccount && !CreditTransactionKeys.Contains(transactionType);

        // DEPOSIT + CREDIT CARD = Balance (owed) Decreases (GOOD Thing)
        // PURCHASE + CHECKING = Balance Decreases (BAD Thing)
        var check2 = !isCreditAccount && CreditTransactionKeys.Contains(transactionType);

        var rslt = check1 || check2;

        return rslt;
    }

    public static TransactionKeys ToTransactionType(string transactionType) =>
        Enum.Parse<TransactionKeys>(transactionType);
}