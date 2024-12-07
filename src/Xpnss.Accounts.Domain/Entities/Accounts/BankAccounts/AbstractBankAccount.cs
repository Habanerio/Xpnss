using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;

public abstract class AbstractBankAccount :
    AbstractAccount
{
    public override AccountEnums.AccountKeys AccountType => AccountEnums.AccountKeys.BANK;

    public override BankAccountEnums.BankAccountKeys BankAccountType { get; } =
        BankAccountEnums.BankAccountKeys.NA;

    public override InvestmentAccountEnums.InvestmentAccountKeys InvestmentAccountType { get; } =
        InvestmentAccountEnums.InvestmentAccountKeys.NA;

    public override LoanAccountEnums.LoanAccountKeys LoanAccountType { get; } =
        LoanAccountEnums.LoanAccountKeys.NA;

    public string BankName { get; set; }


    protected AbstractBankAccount(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,
        string bankName = "",
        string extAcctId = "",
        int? sortOrder = null) :
        base(
            userId,
            accountName,
            description,
            displayColor,
            extAcctId,
            sortOrder)
    {
        BankName = bankName;
    }

    protected AbstractBankAccount(
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
        int sortOrder,
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
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        BankName = bankName;
    }
}