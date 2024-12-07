using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;

public sealed class CreditLineAccount :
    AbstractBankAccount, IHasCreditLimit, IHasInterestRate
{
    public override BankAccountEnums.BankAccountKeys BankAccountType =>
        BankAccountEnums.BankAccountKeys.CREDITLINE;

    public override bool IsCredit => true;


    public Money CreditLimit { get; set; }

    public PercentageRate InterestRate { get; set; }

    public bool IsOverLimit => Balance > CreditLimit;


    private CreditLineAccount(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        string bankName,
        string extAcctId,
        int? sortOrder) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            bankName,
            extAcctId,
            sortOrder)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;

        // Add 'Credit Line Account Created' event
    }

    private CreditLineAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string bankName,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
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
            bankName,
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
    }

    /// <summary>
    /// Creates an instance of a NEW Line of Credit Account.
    /// </summary>
    /// <returns></returns>
    public static CreditLineAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        string bankName = "",
        string extAcctId = "",
        int? sortOrder = null)
    {
        return new CreditLineAccount(
            userId,
            accountName,
            description,
            displayColor,
            creditLimit,
            interestRate,
            bankName,
            extAcctId,
            sortOrder);
    }

    /// <summary>
    /// Creates an instance of a NEW Line of Credit Account.
    /// </summary>
    /// <returns></returns>
    public static CreditLineAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string bankName,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
        Money creditLimit,
        PercentageRate interestRate,
        bool isDefault,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        if (creditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        return new CreditLineAccount(
            id,
            userId,
            accountName,
            balance,
            bankName,
            closedDate,
            description,
            displayColor,
            extAcctId,
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