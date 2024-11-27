using System.ComponentModel.DataAnnotations;

namespace Habanerio.Xpnss.Domain.Types;

public static class TransactionTypes
{
    public enum Keys
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


        /// <summary>
        /// Bonuses credited to the account (e.g., for opening an account).
        /// </summary>
        [Display(Name = "Bonus", Description = "Bonuses credited to the account (e.g., for opening an account).")]
        BONUS,

        // Just a withdrawal?
        /// <summary>
        /// Borrowing money against a credit card or line of credit.
        /// </summary>
        [Display(Description = "Borrowing money against a credit card or line of credit.")]
        CASH_ADVANCE,

        /// <summary>
        /// General term for charges applied to the account (e.g., late fees).
        /// </summary>
        [Display(Name = "Charge", Description = "General term for charges applied to the account (e.g., late fees).")]
        CHARGE_FEE,

        //[Obsolete("Same as Deposit?")]
        //[Display(Name = "Credit", Description = "")]
        //CREDIT,

        [Display(Name = "Deposit", Description = "Adding money to the account (e.g., paycheck, cash deposit).")]
        DEPOSIT,

        [Display(Name = "Dividend Received", Description = "Dividend payments credited to the account.")]
        DIVIDEND_RECEIVED,

        // Can this be a Deposit on a Cash Account, and assigned to an "Income" Category?
        //[Display(Name = "Income", Description = "Money earned from doing work (or selling something)")]
        //INCOME,

        [Display(Name = "Interest Charge", Description = "Interest charges to an account such as a Credit Card, Line of Credit, Loan, ...")]
        INTEREST_CHARGE,

        // Can this be a Deposit on a Savings or Investment Account, and assigned to a "Interest Earned" Category?
        [Display(Description = "Interest credited to the account (e.g., savings or checking account).")]
        INTEREST_EARNED,

        [Display(Description = "")]
        PAYMENT,

        [Display(Name = "Payment In", Description = "A payment made to an account")]
        PAYMENT_IN,

        [Display(Name = "Payment Out", Description = "A payment made from an account")]
        PAYMENT_OUT,

        [Display(Description = "Spending money for goods or services.")]
        PURCHASE,

        [Display(Description = "Receiving a refund for a returned product or overpayment.")]
        REFUND,

        [Display(Description = "Money reimbursed for expenses or payments made on behalf of others.")]
        REIMBURSEMENT,

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

    public static IReadOnlyCollection<Keys> CreditTransactionTypes => new[]
    {
        Keys.ADJUSTMENT_CREDIT,
        //Keys.CREDIT,
        //Keys.BALANCE_TRANSFER,
        Keys.BALANCE_TRANSFER_IN,
        Keys.DEPOSIT,
        Keys.DIVIDEND_RECEIVED,
        //Keys.INCOME,
        Keys.INTEREST_EARNED,
        Keys.PAYMENT,
        //Keys.REFUND,
        Keys.REFUND_REIMBURSEMENT,
        //Keys.REIMBURSEMENT,
        //Keys.TRANSFER,
        Keys.TRANSFER_IN,
    };

    public static IReadOnlyCollection<Keys> DebitTransactionTypes => new[]
    {
        Keys.ADJUSTMENT_DEBIT,
        //Keys.BALANCE_TRANSFER,
        Keys.BALANCE_TRANSFER_OUT,
        Keys.CHARGE_FEE,
        //Keys.CHARGE,
        //Keys.FEE,
        Keys.INTEREST_CHARGE,
        Keys.PAYMENT,
        Keys.PURCHASE,
        //Keys.TRANSFER,
        Keys.TRANSFER_OUT,
        Keys.WITHDRAWAL,
    };

    public static IReadOnlyCollection<Keys> AllTransactionTypes =>
        Enum.GetValues<Keys>();

    public static IReadOnlyCollection<string> AllTransactionTypeStrings() =>
        Enum.GetValues<Keys>().Select(t => t.ToString()).ToList();

    public static IReadOnlyCollection<Keys> UserTransactionTypes() =>
    [
        Keys.BALANCE_TRANSFER,
        //  // Would this just be a "Deposit"?
        //  - Keys.BALANCE_TRANSFER_IN,
        //  - Keys.BALANCE_TRANSFER_OUT,
        // CASH_ADVANCE: If the Credit Account has business logic for this, may be useful?
        // Keys.CASH_ADVANCE,
        Keys.CHARGE_FEE,
        Keys.DEPOSIT,
        // DIVIDEND_RECEIVED: IF there's an Investment Account, then we may need this,
        // instead of putting it under "Deposit" (or "Income") and applying a "Dividend" category?
        Keys.DIVIDEND_RECEIVED,
        // INCOME: Can this be a DEPOSIT and fall under an "INCOME" Category?
        //Keys.INCOME,
        // INTEREST_CHARGE: Can this be a CHARGE_FEE and fall under an "INTEREST" Category?
        // Keys.INTEREST_CHARGE,
        // INTEREST_EARNED: Can this be a DEPOSIT and fall under an "INTEREST" Category? If there's an Investment Account, then we may need this,
        Keys.INTEREST_EARNED,
        Keys.PAYMENT, 
        //  - Keys.PAYMENT_IN,
        //  - Keys.PAYMENT_OUT,
        Keys.PURCHASE,
        Keys.REFUND_REIMBURSEMENT,
        // Is there a difference between the following two? Or group them together?
        //  - Keys.REFUND?,
        //  - Keys.REIMBURSEMENT?,
        Keys.TRANSFER,
        //  - Keys.TRANSFER_IN,
        //  - Keys.TRANSFER_OUT,
        Keys.WITHDRAWAL
    ];

    public static bool IsCreditTransaction(Keys transactionType) =>
          CreditTransactionTypes.Contains(transactionType);

    public static bool IsCreditTransaction(string transactionType) =>
        CreditTransactionTypes.Any(t => t.ToString().Equals(transactionType));

    /// <summary>
    /// Determines whether the account should be credited or debited based on the account type and transaction type.
    /// Credit Account: Balance increases with Credits (purchases) and decreases with Debits (deposits).
    /// Debit Account: Balance increases with Debits (deposits) and decreases with Credits (purchases).
    /// </summary>
    /// <returns></returns>
    public static bool DoesBalanceIncrease(AccountTypes.Keys accountType, Keys transactionType)
    {
        // DEPOSIT + CHECKING = Balance Increases (GOOD Thing)
        // PURCHASE + CREDIT CARD = Balance (owed) Increases (BAD Thing)
        var check1 = AccountTypes.IsCreditAccount(accountType) && !CreditTransactionTypes.Contains(transactionType);

        // DEPOSIT + CREDIT CARD = Balance (owed) Decreases (GOOD Thing)
        // PURCHASE + CHECKING = Balance Decreases (BAD Thing)
        var check2 = !AccountTypes.IsCreditAccount(accountType) && CreditTransactionTypes.Contains(transactionType);

        var rslt = check1 || check2;

        return rslt;
    }

    public static Keys ToTransactionType(string transactionType) =>
        Enum.Parse<Keys>(transactionType);
}