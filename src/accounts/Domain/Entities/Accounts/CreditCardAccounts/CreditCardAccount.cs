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
        int? sortOrder,
        decimal startingBalance,
        DateTime? startingBalanceDate) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            extAcctId,
            isDefault,
            sortOrder, startingBalance, startingBalanceDate)
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
        int? sortOrder = null,
        decimal startingBalance = 0,
        DateTime? startingBalanceDate = null)
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
            sortOrder, startingBalance, startingBalanceDate);
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
        decimal startingBalance,
        DateTime? startingBalanceDate,
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