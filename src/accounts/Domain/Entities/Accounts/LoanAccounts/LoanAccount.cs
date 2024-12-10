using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.LoanAccounts;

public class LoanAccount :
    AbstractAccount, IHasInterestRate, IHasCreditLimit
{
    public override AccountEnums.AccountKeys AccountType =>
        AccountEnums.AccountKeys.LOAN;

    public override BankAccountEnums.BankAccountKeys BankAccountType =>
        BankAccountEnums.BankAccountKeys.NA;

    public override InvestmentAccountEnums.InvestmentAccountKeys InvestmentAccountType =>
        InvestmentAccountEnums.InvestmentAccountKeys.NA;

    /// <summary>
    /// LoanAcctType
    /// </summary>
    public override LoanAccountEnums.LoanAccountKeys LoanAccountType { get; }

    public override bool IsCredit => true;


    public Money CreditLimit { get; set; }

    public string InstitutionName { get; private set; }

    public PercentageRate InterestRate { get; set; }

    public bool IsOverLimit => Balance > CreditLimit;

    public bool IsPaidOff => DatePaidOff.HasValue;

    /// <summary>
    /// The creditLimit of interest that has been paid on the loan since the start date,
    /// using the original creditLimit of the loan, and NOT the remaining balance.
    /// </summary>
    public decimal InterestPaid
    {
        get
        {
            TimeSpan timeElapsed = DateTime.Now.Date - DateTermStarted.Date;
            double totalDays = timeElapsed.TotalDays;

            // Interest is applied to the original creditLimit, and not the remaining balance.
            decimal interestPaid = (decimal)totalDays * (InterestRate / 365) * CreditLimit.Value;
            return interestPaid;
        }
    }

    // LoanAcctId

    /// <summary>
    /// The number of months that the loan is for.
    /// </summary>
    public int TermMonths { get; set; }

    /// <summary>
    /// The date that the loan starts.
    /// </summary>
    public DateTime DateTermStarted { get; set; }

    public DateTime DateTermEnds { get; set; }

    public DateTime? DatePaidOff { get; set; }

    private LoanAccount(
        UserId userId,
        LoanAccountEnums.LoanAccountKeys loanAcctType,
        AccountName accountName,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        string institutionName,
        string loanAcctId,
        bool isDefault,
        int? sortOrder) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            loanAcctId,
            isDefault,
            sortOrder)
    {
        LoanAccountType = loanAcctType;

        CreditLimit = creditLimit;
        InterestRate = interestRate;

        InstitutionName = institutionName;
    }

    private LoanAccount(
        AccountId id,
        UserId userId,
        LoanAccountEnums.LoanAccountKeys loanAcctType,
        AccountName accountName,
        Money balance,
        DateTime? closedDate,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        string institutionName,
        string loanAcctId,
        bool isDefault,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted) :
        base(
            id,
            userId,
            accountName,
            balance,
            closedDate,
            description,
            displayColor,
            loanAcctId,
            isDefault,
            sortOrder,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        LoanAccountType = loanAcctType;

        CreditLimit = creditLimit;
        InterestRate = interestRate;

        InstitutionName = institutionName;
    }

    public static LoanAccount New(
        UserId userId,
        LoanAccountEnums.LoanAccountKeys loanType,
        AccountName accountName,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        string institutionName = "",
        string loanAcctId = "",
        bool isDefault = false,
        int? sortOrder = null)
    {
        return new LoanAccount(
            userId,
            LoanAccountEnums.LoanAccountKeys.AUTO_LOAN,
            accountName,
            description,
            displayColor,
            creditLimit,
            interestRate,
            institutionName,
            loanAcctId,
            isDefault,
            sortOrder);
    }

    /// <summary>
    /// Creates an instance of a NEW Line of Credit Account.
    /// </summary>
    /// <returns></returns>
    public static LoanAccount Load(
        AccountId id,
        UserId userId,
        LoanAccountEnums.LoanAccountKeys loanType,
        AccountName accountName,
        Money balance,
        DateTime? closedDate,
        string description,
        string displayColor,
        string institutionName,
        string loanAcctId,
        Money creditLimit,
        PercentageRate interestRate,
        bool isDefault,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateDeleted,
        DateTime? dateUpdated)
    {
        if (creditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        return new LoanAccount(
            id,
            userId,
            loanType,
            accountName,
            balance,
            closedDate,
            description,
            displayColor,
            creditLimit,
            interestRate,
            institutionName,
            loanAcctId,
            isDefault,
            sortOrder,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }



    /// <summary>
    /// Applies a Transaction CreditLimit to the Account's Balance.<br />
    /// When the Transaction is a Credit, the creditLimit is added to the Balance.<br />
    /// When the Transaction is a Debit, the creditLimit is subtracted from the Balance.
    /// </summary>
    /// <param name="amount">The creditLimit of the Transaction</param>
    /// <param name="transactionType">The type of Transaction that occurred</param>
    public override void AddTransactionAmount(Money amount, TransactionEnums.TransactionKeys transactionType)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot add a transaction to a deleted Account");

        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), $"AddTransactionAmount value cannot be negative ({amount})");

        // For default Credit Accounts
        if (TransactionEnums.IsCreditTransaction(transactionType))
        {
            Balance -= amount;
        }
        else
        {
            Balance += amount;
        }
    }

    /// <summary>
    /// Undoes a previously applied Transaction CreditLimit from the Account's Balance (eg: for when a Transaction is deleted).<br />
    /// When the Transaction is a Credit, the creditLimit will be SUBTRACTED from the Balance.<br />
    /// When the Transaction is a Debit, the creditLimit will be ADDED to the Balance.
    /// </summary>
    /// <param name="amount">The creditLimit of the original Transaction</param>
    /// <param name="transactionType">The original Transaction Type</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public override void RemoveTransactionAmount(Money amount, TransactionEnums.TransactionKeys transactionType)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot remove a transaction from a deleted Account");

        if (amount.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), $"RemoveTransaction value cannot be negative ({amount})");

        if (TransactionEnums.IsCreditTransaction(transactionType))
        {
            Balance += amount;
        }
        else
        {
            Balance -= amount;
        }
    }


    public void UpdateInterestRate(PercentageRate newInterestRate)
    {
        throw new NotImplementedException();
    }


    public void UpdateCreditLimit(Money newCreditLimit)
    {
        throw new NotImplementedException();
    }
}