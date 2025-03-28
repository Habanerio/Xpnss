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
        string bankName,
        string extAcctId,
        bool isDefault,
        int? sortOrder,
        decimal startingBalance,
        DateTime? startingBalanceDate) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            bankName,
            extAcctId,
            isDefault,
            sortOrder, startingBalance, startingBalanceDate)
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
        int? sortOrder = null,
        decimal startingBalance = 0,
        DateTime? startingBalanceDate = null)
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
            sortOrder, startingBalance, startingBalanceDate);
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
        decimal startingBalance,
        DateTime? startingBalanceDate,
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
            startingBalance,
            startingBalanceDate,
            dateCreated,
            dateUpdated,
            dateDeleted);
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