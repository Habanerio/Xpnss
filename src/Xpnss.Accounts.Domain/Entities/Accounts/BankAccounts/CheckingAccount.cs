using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;

public sealed class CheckingAccount :
    AbstractBankAccount, IHasOverdraftAmount
{
    public override BankAccountEnums.BankAccountKeys BankAccountType =>
        BankAccountEnums.BankAccountKeys.CHECKING;

    public override bool IsCredit => false;


    public Money OverdraftLimit { get; private set; }

    public bool IsOverLimit => Balance.Value < -1 * OverdraftLimit.Value;

    private CheckingAccount(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        Money overDraftLimit = default,
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
        OverdraftLimit = overDraftLimit;

        // Add 'Checking Account Created' event
    }

    private CheckingAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string bankName,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
        bool isDefault,
        Money overDraftLimit,
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
        OverdraftLimit = overDraftLimit;
    }

    /// <summary>
    /// Creates an instance of a NEW Checking Account.
    /// </summary>
    /// <returns></returns>
    public static CheckingAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        Money overDraftLimit = default,
        string bankName = "",
        string extAcctId = "",
        bool isDefault = false,
        int? sortOrder = null)

    {
        if (overDraftLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(overDraftLimit));

        return new CheckingAccount(
            userId,
            accountName,
            description,
            displayColor,
            overDraftLimit,
            bankName,
            extAcctId,
            isDefault,
            sortOrder);
    }

    /// <summary>
    /// Creates an instance of an EXISTING Checking Account.
    /// </summary>
    /// <returns></returns>
    public static CheckingAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string bankName,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
        bool isDefault,
        Money overDraftAmount,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        if (overDraftAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(overDraftAmount));

        return new CheckingAccount(
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
            overDraftAmount,
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
    /// This updates the current Overdraft CreditLimit of the Account.
    /// This is for when the current Overdraft CreditLimit is out of sync with reality.
    /// Use this _AFTER_ adding it to the Adjustments (Domain Event).
    /// </summary>
    /// <param name="newOverdraftAmount">The new value for the current OverdraftA mount</param>
    /// <exception cref="InvalidOperationException">When the account is marked as deleted</exception>
    /// <exception cref="ArgumentOutOfRangeException">When the new Overdraft CreditLimit is below 0</exception>
    /// <remarks>Would prefer this to be `internal`.</remarks>
    public void UpdateOverdraftAmount(Money newOverdraftAmount)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted Account");

        if (newOverdraftAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(newOverdraftAmount));

        OverdraftLimit = newOverdraftAmount;
        DateUpdated = DateTime.UtcNow;
    }
}