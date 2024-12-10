using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CreditCardAccounts;

public sealed class CreditCardAccount :
    AbstractAccount, IHasCreditLimit, IHasInterestRate
{
    public override AccountEnums.AccountKeys AccountType =>
        AccountEnums.AccountKeys.CREDITCARD;

    public override BankAccountEnums.BankAccountKeys BankAccountType =>
        BankAccountEnums.BankAccountKeys.NA;

    public override InvestmentAccountEnums.InvestmentAccountKeys InvestmentAccountType =>
    InvestmentAccountEnums.InvestmentAccountKeys.NA;

    public override LoanAccountEnums.LoanAccountKeys LoanAccountType =>
        LoanAccountEnums.LoanAccountKeys.NA;

    //TODO: This and BankName could probably be moved to the AbstractAccount (with ExtAcctId)
    //      and InstitutionName could just return underlying value
    public string InstitutionName { get; set; }

    public override bool IsCredit => true;

    public Money CreditLimit { get; set; }

    public PercentageRate InterestRate { get; set; }


    public bool IsOverLimit => Balance > CreditLimit;


    private CreditCardAccount(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        string extAcctId,
        string institutionName,
        bool isDefault,
        int? sortOrder) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            extAcctId,
            isDefault,
            sortOrder)
    {
        if (creditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit), "Credit Limit cannot be negative");

        CreditLimit = creditLimit;
        InterestRate = interestRate;

        InstitutionName = institutionName;

        // Add 'Credit Card Account Created' event
    }

    private CreditCardAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
        string institutionName,
        Money creditLimit,
        PercentageRate interestRate,
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
            extAcctId,
            isDefault,
            sortOrder,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;

        InstitutionName = institutionName;
    }

    /// <summary>
    /// Creates an instance of a NEW Credit Card Account.
    /// </summary>
    /// <returns></returns>
    public static CreditCardAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        string institutionName = "",
        string extAcctId = "",
        bool isDefault = false,
        int? sortOrder = null)
    {
        return new CreditCardAccount(
            userId,
            accountName,
            description,
            displayColor,
            creditLimit,
            interestRate,
            extAcctId,
            institutionName,
            isDefault,
            sortOrder);
    }

    /// <summary>
    /// Creates an instance of a NEW Credit Card Account.
    /// </summary>
    /// <returns></returns>
    public static CreditCardAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
        string institutionName,
        Money creditLimit,
        PercentageRate interestRate,
        bool isDefault,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new CreditCardAccount(
            id,
            userId,
            accountName,
            balance,
            closedDate,
            description,
            displayColor,
            extAcctId,
            institutionName,
            creditLimit,
            interestRate,
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

    public void UpdateCreditLimit(Money newCreditLimit)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update Credit Limit from a deleted Account");

        CreditLimit = newCreditLimit;
        DateUpdated = DateTime.UtcNow;
    }


    public void UpdateInterestRate(PercentageRate newInterestRate)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update Credit Limit from a deleted Account");

        InterestRate = newInterestRate;
        DateUpdated = DateTime.UtcNow;
    }
}