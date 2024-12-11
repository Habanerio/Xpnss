using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Shared.Types;

public static class TransactionEnums
{
    public enum TransactionKeys
    {
        [JsonPropertyName("Adjustment")]
        [Display(Name = "Adjustment", Description = "Adjustment to increase or decrease the balance due to a correction by the institution")]
        ADJUSTMENT,

        /// <summary>
        /// Adjustments to increase the balance (e.g., bank error correction).
        /// </summary>
        [JsonPropertyName("Credit Adjustment")]
        [Display(Name = "Adjustment Credit", Description = "Adjustments to increase the balance (e.g., bank error correction).")]
        ADJUSTMENT_CREDIT,

        /// <summary>
        /// Adjustments to decrease the balance (e.g., bank error correction).
        /// </summary>
        [JsonPropertyName("Debit Adjustment")]
        [Display(Name = "Adjustment Debit", Description = "Adjustments to decrease the balance (e.g., bank error correction).")]
        ADJUSTMENT_DEBIT,

        /// <summary>
        /// This should be from the source of the transfer, and not the destination
        /// </summary>
        [JsonPropertyName("Balance Transfer")]
        [Display(Name = "Balance Transfer", Description = "Specific type of transfer between two accounts. Usually from a Credit Card to some other ofxAccount")]
        BALANCE_TRANSFER,

        // Not sure if this is needed? It's just a Deposit?
        [JsonPropertyName("Balance Transfer In")]
        [Display(Name = "Balance Transfer In", Description = "Money transferred into the ofxAccount from another ofxAccount.")]
        BALANCE_TRANSFER_IN,

        [JsonPropertyName("Balance Transfer Out")]
        [Display(Name = "Balance Transfer Out", Description = "Money transferred out of the ofxAccount to another ofxAccount.")]
        BALANCE_TRANSFER_OUT,


        ///// <summary>
        ///// Bonuses credited to the ofxAccount (e.g., for opening an ofxAccount).
        ///// </summary>
        //[Display(Name = "Bonus", Description = "Bonuses credited to the ofxAccount (e.g., for opening an ofxAccount).")]
        //BONUS,

        // Just a withdrawal?
        /// <summary>
        /// Borrowing money against a credit card or line of credit.
        /// </summary>
        [JsonPropertyName("Cash Advance")]
        [Display(Description = "Borrowing money against a credit card or line of credit.")]
        CASH_ADVANCE,

        //[Obsolete("Same as Deposit?")]
        //[Display(Name = "Credit", Description = "")]
        //CREDIT,

        [JsonPropertyName("Deposit")]
        [Display(Name = "Deposit", Description = "Adding money to the ofxAccount (e.g., paycheck, cash deposit).")]
        DEPOSIT,

        [JsonPropertyName("Dividend")]
        [Display(Name = "Dividend Received", Description = "Dividend payments credited to the ofxAccount.")]
        DIVIDEND,

        /// <summary>
        /// General term for charges applied to the ofxAccount (e.g., late fees).
        /// </summary>
        [JsonPropertyName("Fee")]
        [Display(Name = "Fee", Description = "General term for charges applied to the ofxAccount (e.g., late fees).")]
        CHARGE_FEE,

        // Can this be a Deposit on a Cash Account, and assigned to an "Income" Category?
        //[Display(Name = "Income", Description = "Money earned from doing work (or selling something)")]
        //INCOME,
        [JsonPropertyName("Interest Charge")]
        [Display(Name = "Interest Charge", Description = "Interest charges to an ofxAccount such as a Credit Card, Line of Credit, Loan, ...")]
        INTEREST_CHARGE,

        // Can this be a Deposit on a Savings or Investment Account, and assigned to a "Interest Earned" Category?
        [JsonPropertyName("Interest Earned")]
        [Display(Description = "Interest credited to the ofxAccount (e.g., savings or checking ofxAccount).")]
        INTEREST_EARNED,

        [JsonPropertyName("Other ???")]
        [Display(Name = "Other", Description = "For all other types of transactions.")]
        OTHER,

        /// <summary>
        /// This should be from the source of the transfer, and not the destination
        /// </summary>
        [JsonPropertyName("Payment")]
        [Display(Description = "")]
        PAYMENT,

        [JsonPropertyName("Payment In")]
        [Display(Name = "Payment In", Description = "A payment made to an ofxAccount")]
        PAYMENT_IN,

        [JsonPropertyName("Payment Out")]
        [Display(Name = "Payment Out", Description = "A payment made from an ofxAccount")]
        PAYMENT_OUT,

        [JsonPropertyName("Purchase")]
        [Display(Description = "Spending money for goods or services.")]
        PURCHASE,

        [JsonPropertyName("Refund / Reimbursement")]
        [Display(Name = "Refund / Reimbursement", Description = "Money received from a prior purchase or from money lent")]
        REFUND_REIMBURSEMENT,

        /// <summary>
        /// This should be from the source of the transfer, and not the destination
        /// </summary>
        [JsonPropertyName("Transfer")]
        [Display(Name = "Transfer", Description = "The action the user would take to Transfer Out of one ofxAccount and Transfer In to another ofxAccount")]
        TRANSFER,

        // This would be the actual Transfer Transaction on the ofxAccount that is receiving
        [JsonPropertyName("Transfer In")]
        [Display(Name = "Transfer In", Description = "Money transferred into the ofxAccount from another ofxAccount.")]
        TRANSFER_IN,

        // This would be the actual Transfer Transaction on the ofxAccount that is paying
        [JsonPropertyName("Transfer Out")]
        [Display(Name = "Transfer Out", Description = "Money transferred out of the ofxAccount to another ofxAccount.")]
        TRANSFER_OUT,

        /// <summary>
        /// This should be from the source of the transfer, and not the destination.
        /// The destination (if one is provided) would be a deposit (to Cash/Wallet).
        /// </summary>
        [JsonPropertyName("Withdrawal")]
        [Display(Description = "Taking money out of the ofxAccount (e.g., ATM or cash withdrawal).")]
        WITHDRAWAL,
    }

    public static Dictionary<int, string> ToDictionary()
    {
        return Enum.GetValues<TransactionKeys>()
            .ToDictionary(k => (int)k, v => v.ToString().Replace("_", " "));
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
        TransactionKeys.BALANCE_TRANSFER,
        TransactionKeys.BALANCE_TRANSFER_OUT,
        TransactionKeys.CHARGE_FEE,
        TransactionKeys.INTEREST_CHARGE,
        TransactionKeys.PAYMENT,
        TransactionKeys.PAYMENT_OUT,
        TransactionKeys.PURCHASE,
        TransactionKeys.TRANSFER,
        TransactionKeys.TRANSFER_OUT,
        TransactionKeys.WITHDRAWAL,
    };

    public static IReadOnlyDictionary<int, string> CreditTransactionTypes => new Dictionary<int, string>()
    {
        {(int) TransactionKeys.ADJUSTMENT_CREDIT, TransactionKeys.ADJUSTMENT_CREDIT.ToString().Replace("_", " ")},
        { (int) TransactionKeys.BALANCE_TRANSFER_IN, TransactionKeys.BALANCE_TRANSFER_IN.ToString().Replace("_", " ")},
        {(int) TransactionKeys.DEPOSIT, TransactionKeys.DEPOSIT.ToString().Replace("_", " ")},
        {(int) TransactionKeys.DIVIDEND, TransactionKeys.DIVIDEND.ToString().Replace("_", " ")},
        {(int) TransactionKeys.INTEREST_EARNED, TransactionKeys.INTEREST_EARNED.ToString().Replace("_", " ")},
        {(int) TransactionKeys.PAYMENT_IN, TransactionKeys.PAYMENT_IN.ToString().Replace("_", " ")},
        {(int) TransactionKeys.REFUND_REIMBURSEMENT, TransactionKeys.REFUND_REIMBURSEMENT.ToString().Replace("_", " ")},
        {(int) TransactionKeys.TRANSFER_IN, TransactionKeys.TRANSFER_IN.ToString().Replace("_", " ")}
    };

    public static IReadOnlyDictionary<int, string> DebitTransactionTypes => new Dictionary<int, string>()
    {
        { (int)TransactionKeys.ADJUSTMENT_DEBIT, TransactionKeys.ADJUSTMENT_DEBIT.ToString().Replace("_", " ")},
        { (int) TransactionKeys.BALANCE_TRANSFER_OUT, TransactionKeys.BALANCE_TRANSFER_OUT.ToString().Replace("_", " ")},
        { (int) TransactionKeys.CHARGE_FEE, TransactionKeys.CHARGE_FEE.ToString().Replace("_", " ")},
        { (int) TransactionKeys.INTEREST_CHARGE, TransactionKeys.INTEREST_CHARGE.ToString().Replace("_", " ")},
        { (int) TransactionKeys.PAYMENT_OUT, TransactionKeys.PAYMENT_OUT.ToString().Replace("_", " ")},
        { (int) TransactionKeys.PURCHASE, TransactionKeys.PURCHASE.ToString().Replace("_", " ")},
        { (int) TransactionKeys.TRANSFER_OUT, TransactionKeys.TRANSFER_OUT.ToString().Replace("_", " ")},
        { (int) TransactionKeys.WITHDRAWAL, TransactionKeys.WITHDRAWAL.ToString().Replace("_", " ")}
    };

    public static IReadOnlyCollection<TransactionKeys> AllTransactionKeys =>
        Enum.GetValues<TransactionKeys>();

    public static IReadOnlyCollection<string> AllTransactionKeyStrings() =>
        Enum.GetValues<TransactionKeys>().Select(t => t.ToString()).ToList();

    public static IReadOnlyDictionary<TransactionKeys, bool> UserTransactionTypes =>
        new Dictionary<TransactionKeys, bool>()
        {
            { TransactionKeys.BALANCE_TRANSFER, false },
            { TransactionKeys.CHARGE_FEE, false },
            { TransactionKeys.DEPOSIT, true },
            { TransactionKeys.DIVIDEND, true },
            { TransactionKeys.INTEREST_EARNED, true },
            { TransactionKeys.PAYMENT, false },
            { TransactionKeys.PURCHASE, false },
            { TransactionKeys.REFUND_REIMBURSEMENT, true },
            { TransactionKeys.TRANSFER, false },
            { TransactionKeys.WITHDRAWAL, false }
        };

    //public static IReadOnlyCollection<TransactionKeys> UserTransactionTypes() =>
    //[
    //    TransactionKeys.BALANCE_TRANSFER,
    //    //  // Would this just be a "Deposit"?
    //    //  - AccountEnums.CurrencyKeys.BALANCE_TRANSFER_IN,
    //    //  - AccountEnums.CurrencyKeys.BALANCE_TRANSFER_OUT,
    //    // CASH_ADVANCE: If the Credit Account has business logic for this, may be useful?
    //    // AccountEnums.CurrencyKeys.CASH_ADVANCE,
    //    TransactionKeys.CHARGE_FEE,
    //    TransactionKeys.DEPOSIT,
    //    // DIVIDEND: IF there's an Investment Account, then we may need this,
    //    // instead of putting it under "Deposit" (or "Income") and applying a "Dividend" category?
    //    TransactionKeys.DIVIDEND,
    //    // INCOME: Can this be a DEPOSIT and fall under an "INCOME" Category?
    //    //AccountEnums.CurrencyKeys.INCOME,
    //    // INTEREST_CHARGE: Can this be a SERVICE_FEE and fall under an "INTEREST" Category?
    //    // AccountEnums.CurrencyKeys.INTEREST_CHARGE,
    //    // INTEREST_EARNED: Can this be a DEPOSIT and fall under an "INTEREST" Category? If there's an Investment Account, then we may need this,
    //    TransactionKeys.INTEREST_EARNED,
    //    TransactionKeys.PAYMENT, 
    //    //  - AccountEnums.CurrencyKeys.PAYMENT_IN,
    //    //  - AccountEnums.CurrencyKeys.PAYMENT_OUT,
    //    TransactionKeys.PURCHASE,
    //    TransactionKeys.REFUND_REIMBURSEMENT,
    //    // Is there a difference between the following two? Or group them together?
    //    //  - AccountEnums.CurrencyKeys.REFUND?,
    //    //  - AccountEnums.CurrencyKeys.REIMBURSEMENT?,
    //    TransactionKeys.TRANSFER,
    //    //  - AccountEnums.CurrencyKeys.TRANSFER_IN,
    //    //  - AccountEnums.CurrencyKeys.TRANSFER_OUT,
    //    TransactionKeys.WITHDRAWAL
    //];

    public static bool IsCreditTransaction(TransactionKeys transactionType) =>
          CreditTransactionKeys.Contains(transactionType);

    public static bool IsCreditTransaction(string transactionType) =>
        CreditTransactionKeys.Any(t => t.ToString().Equals(transactionType));

    /// <summary>
    /// Determines whether the ofxAccount should be credited or debited based on the ofxAccount type and transaction type.
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