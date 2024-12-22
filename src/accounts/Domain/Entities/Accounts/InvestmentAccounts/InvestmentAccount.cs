using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.InvestmentAccounts;

public class InvestmentAccount :
    AbstractAccount
{
    public override AccountEnums.AccountKeys AccountType =>
        AccountEnums.AccountKeys.INVESTMENT;

    public override BankAccountEnums.BankAccountKeys BankAccountType =>
        BankAccountEnums.BankAccountKeys.NA;

    public override InvestmentAccountEnums.InvestmentAccountKeys InvestmentAccountType =>
        InvestmentAccountEnums.InvestmentAccountKeys.UNKNOWN;

    public override LoanAccountEnums.LoanAccountKeys LoanAccountType =>
    LoanAccountEnums.LoanAccountKeys.NA;

    public string InstitutionName { get; set; }

    public override bool IsCredit => false;

    /// <summary>
    /// BROKERID (BrokerIdType) - Use ExtAcctId?
    /// </summary>
    //public string BrokerId { get; set; }

    private InvestmentAccount(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
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
        InstitutionName = institutionName;

        // Add `New Investment Account` event
    }

    private InvestmentAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
        string institutionName,
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
        InstitutionName = institutionName;
    }

    public static InvestmentAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        string extAcctId = "",
        string institutionName = "",
        bool isDefault = false,
        int? sortOrder = null,
        decimal startingBalance = 0,
        DateTime? startingBalanceDate = null)
    {
        return new InvestmentAccount(
            userId,
            accountName,
            description,
            displayColor,
            extAcctId,
            institutionName,
            isDefault,
            sortOrder, startingBalance, startingBalanceDate);
    }

    public static InvestmentAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        DateTime? closedDate,
        string description,
        string displayColor,
        string extAcctId,
        string institutionName,
        bool isDefault,
        int sortOrder,
        decimal startingBalance,
        DateTime? startingBalanceDate,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new InvestmentAccount(
            id,
            userId,
            accountName,
            balance,
            closedDate,
            description,
            displayColor,
            extAcctId,
            institutionName,
            isDefault,
            sortOrder,
            startingBalance,
            startingBalanceDate,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }
}