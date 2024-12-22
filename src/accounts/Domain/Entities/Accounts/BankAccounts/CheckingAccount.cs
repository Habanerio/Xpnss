using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

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
        int? sortOrder = null,
        decimal startingBalance = 0,
        DateTime? startingBalanceDate = null) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            bankName,
            extAcctId,
            isDefault,
            sortOrder,
            startingBalance,
            startingBalanceDate)
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
        decimal startingBalance,
        DateTime? startingBalanceDate,
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
            startingBalance,
            startingBalanceDate,
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
        int? sortOrder = null,
        decimal startingBalance = 0,
        DateTime? startingBalanceDate = null)

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
            sortOrder,
            startingBalance,
            startingBalanceDate);
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
        Money overDraftLimit,
        int sortOrder,
        decimal startingBalance,
        DateTime? startingBalanceDate,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        if (overDraftLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(overDraftLimit));

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
            overDraftLimit,
            sortOrder,
            startingBalance,
            startingBalanceDate,
            dateCreated,
            dateUpdated,
            dateDeleted);
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