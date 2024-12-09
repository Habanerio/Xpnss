using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CashAccounts;

public sealed class CashAccount : AbstractAccountBase
{
    public override AccountEnums.AccountKeys AccountType =>
        AccountEnums.AccountKeys.CASH;

    public override BankAccountEnums.BankAccountKeys BankAccountType =>
        BankAccountEnums.BankAccountKeys.NA;

    public override InvestmentAccountEnums.InvestmentAccountKeys InvestmentAccountType =>
        InvestmentAccountEnums.InvestmentAccountKeys.NA;

    public override LoanAccountEnums.LoanAccountKeys LoanAccountType =>
        LoanAccountEnums.LoanAccountKeys.NA;

    public override bool CanBeDeleted => false;

    public override bool IsCredit => false;


    // New Cash Accounts
    private CashAccount(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        bool isDefault,
        int? sortOrder) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            isDefault,
            sortOrder)
    { }

    // Existing Cash Accounts
    private CashAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
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
            description,
            displayColor,
            isDefault,
            sortOrder,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }

    public static CashAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        bool isDefault = false,
        int? sortOrder = null)
    {
        return new CashAccount(
            userId,
            accountName,
            description,
            displayColor,
            isDefault,
            sortOrder);
    }

    public static CashAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        bool isDefault,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new CashAccount(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,
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
}