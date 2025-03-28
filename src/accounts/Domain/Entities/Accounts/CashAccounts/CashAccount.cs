using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

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
        int? sortOrder,
        decimal startingBalance,
        DateTime? startingBalanceDate) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            isDefault,
            sortOrder, startingBalance, startingBalanceDate)
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
            description,
            displayColor,
            isDefault,
            sortOrder,
            startingBalance,
            startingBalanceDate,
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
        int? sortOrder = null,
        decimal startingBalance = 0,
        DateTime? startingBalanceDate = null)
    {
        return new CashAccount(
            userId,
            accountName,
            description,
            displayColor,
            isDefault,
            sortOrder, startingBalance, startingBalanceDate);
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
        decimal startingBalance,
        DateTime? startingBalanceDate,
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
            startingBalance,
            startingBalanceDate,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }
}