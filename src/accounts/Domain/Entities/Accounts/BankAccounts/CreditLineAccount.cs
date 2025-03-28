using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

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
        bool isDefault = false,
        int? sortOrder = null,
        decimal startingBalance = 0,
        DateTime? startingBalanceDate = null)
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
            isDefault,
            sortOrder, startingBalance, startingBalanceDate);
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
        decimal startingBalance,
        DateTime? startingBalanceDate,
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
            startingBalance,
            startingBalanceDate,
            dateCreated,
            dateUpdated,
            dateDeleted);
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