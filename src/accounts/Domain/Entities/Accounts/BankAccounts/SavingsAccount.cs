using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;

public sealed class SavingsAccount :
    AbstractBankAccount, IHasInterestRate
{
    public override BankAccountEnums.BankAccountKeys BankAccountType =>
        BankAccountEnums.BankAccountKeys.SAVINGS;

    public override bool IsCredit => false;


    public PercentageRate InterestRate { get; set; }


    /// <summary>
    /// New Savings Account
    /// </summary>
    private SavingsAccount(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        PercentageRate interestRate,
        string bankName = "",
        string extAcctId = "",
        bool isDefault = false,
        int? sortOrder = null) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            bankName,
            extAcctId,
            isDefault,
            sortOrder)
    {
        InterestRate = interestRate;
    }

    /// <summary>
    /// Existing Savings Account
    /// </summary>
    private SavingsAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string bankName,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
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
        InterestRate = interestRate;
    }

    /// <summary>
    /// Creates an instance of a NEW Savings Account.
    /// </summary>
    /// <returns></returns>
    public static SavingsAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        PercentageRate interestRate,
        string bankName = "",
        string extAcctId = "",
        bool isDefault = false,
        int? sortOrder = null)
    {
        return new SavingsAccount(
            userId,
            accountName,
            description,
            displayColor,
            interestRate,
            bankName,
            extAcctId,
            isDefault,
            sortOrder);
    }

    /// <summary>
    /// Creates an instance of an EXISTING Savings Account.
    /// </summary>
    /// <returns></returns>
    public static SavingsAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string bankName,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
        PercentageRate interestRate,
        bool isDefault,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new SavingsAccount(
            id,
            userId,
            accountName,
            balance,
            bankName,
            closedDate,
            description,
            displayColor,
            extAcctId,
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
            Balance += amount;
        }
        else
        {
            Balance -= amount;
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
            Balance -= amount;
        }
        else
        {
            Balance += amount;
        }
    }


    /// <summary>
    /// This updates the current Interest Rate of the Account.
    /// This is for when the current Interest Rate is out of sync with reality.
    /// Use this _AFTER_ adding it to the Adjustments (Domain Event).
    /// </summary>
    /// <param name="newInterestRate">The new value for the current Interest Rate</param>
    /// <remarks>Would prefer this to be `internal`.</remarks>
    public void UpdateInterestRate(PercentageRate newInterestRate)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted Account");

        InterestRate = newInterestRate;

        throw new NotImplementedException();
    }
}