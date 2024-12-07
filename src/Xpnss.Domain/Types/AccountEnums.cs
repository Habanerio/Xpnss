namespace Habanerio.Xpnss.Domain.Types;

//public static class InvestmentAccountEnums
//{
//    public enum AccountEnums
//    {
//        PERSONAL,
//        BUSINESS,
//        CORPORATE,
//        OTHER,

//        UNKNOWN = 999
//    }
//}

/// <summary>
/// Abstract Account Types
/// </summary>
public static class AccountEnums
{
    public enum AccountKeys
    {
        BANK = 0,
        CREDITCARD,
        INVESTMENT,
        LOAN,
        PRESENTMENT,

        CASH = 100,

        UNKNOWN = 999
    }
}

/// <summary>
/// OFX: AccountEnum
/// </summary>
public static class BankAccountEnums
{
    public enum BankAccountKeys
    {
        NA = -1,

        CHECKING,
        SAVINGS,
        MONEYMRKT,
        CREDITLINE,
        CD,

        UNKNOWN = 999
    }
}

/// <summary>
/// OFX: InvestmentAccountEnum
/// </summary>
public static class InvestmentAccountEnums
{
    public enum InvestmentAccountKeys
    {
        NA = -1,

        INDIVIDUAL,
        JOINT,
        TRUST,
        CORPORATE,

        UNKNOWN = 999
    }
}

/// <summary>
/// OFX: LoanAccountEnum
/// </summary>
public static class LoanAccountEnums
{
    public enum LoanAccountKeys
    {
        NA,

        AUTO_LOAN,
        CONSUMER_LOAN,
        MORTGAGE_LOAN,
        COMMERCIAL_LOAN,
        STUDENT_LOAN,
        MILITARY_LOAN,
        SMALL_BUSINESS,
        CONSTRUCTION,
        HOME_EQUITY,

        UNKNOWN = 999
    }
}


public static class AllAccountEnums
{
    public enum Keys
    {
        CASH = 0,
        CHECKING,
        SAVINGS,
        CREDITLINE,
        CREDITCARD,
        INVESTMENT,
        LOAN,

        UNKNOWN = 999
    }

    public static
        (AccountEnums.AccountKeys AccountType,
        BankAccountEnums.BankAccountKeys BankType,
        InvestmentAccountEnums.InvestmentAccountKeys InvestmentType,
        LoanAccountEnums.LoanAccountKeys LoanType) GetTypes(Keys key)
    {
        if (key == Keys.CASH)
        {
            return (
                AccountEnums.AccountKeys.CASH,
                BankAccountEnums.BankAccountKeys.NA,
                InvestmentAccountEnums.InvestmentAccountKeys.NA,
                LoanAccountEnums.LoanAccountKeys.NA);
        }

        if (key == Keys.CHECKING)
        {
            return (
                AccountEnums.AccountKeys.BANK,
                BankAccountEnums.BankAccountKeys.CHECKING,
                InvestmentAccountEnums.InvestmentAccountKeys.NA,
                LoanAccountEnums.LoanAccountKeys.NA);
        }

        if (key == Keys.SAVINGS)
        {
            return (
                AccountEnums.AccountKeys.BANK,
                BankAccountEnums.BankAccountKeys.SAVINGS,
                InvestmentAccountEnums.InvestmentAccountKeys.NA,
                LoanAccountEnums.LoanAccountKeys.NA);
        }

        if (key == Keys.CREDITLINE)
        {
            return (
                AccountEnums.AccountKeys.BANK,
                BankAccountEnums.BankAccountKeys.CREDITLINE,
                InvestmentAccountEnums.InvestmentAccountKeys.NA,
                LoanAccountEnums.LoanAccountKeys.NA);
        }

        if (key == Keys.CREDITCARD)
        {
            return (
                AccountEnums.AccountKeys.CREDITCARD,
                BankAccountEnums.BankAccountKeys.NA,
                InvestmentAccountEnums.InvestmentAccountKeys.NA,
                LoanAccountEnums.LoanAccountKeys.NA);
        }

        if (key == Keys.INVESTMENT)
        {
            return (
                AccountEnums.AccountKeys.INVESTMENT,
                BankAccountEnums.BankAccountKeys.NA,
                InvestmentAccountEnums.InvestmentAccountKeys.UNKNOWN,
                LoanAccountEnums.LoanAccountKeys.NA);
        }

        if (key == Keys.LOAN)
        {
            return (
                AccountEnums.AccountKeys.LOAN,
                BankAccountEnums.BankAccountKeys.NA,
                InvestmentAccountEnums.InvestmentAccountKeys.NA,
                LoanAccountEnums.LoanAccountKeys.UNKNOWN);
        }

        throw new InvalidOperationException($"'{key}' is an unknown type");
    }

    // Could make this a generic
    public static Dictionary<int, string> ToDictionary()
    {
        return Enum.GetValues<AllAccountEnums.Keys>()
            .ToDictionary(k => (int)k, v => v.ToString());
    }
}