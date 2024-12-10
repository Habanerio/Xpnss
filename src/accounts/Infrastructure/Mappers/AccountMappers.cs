using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CashAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CreditCardAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.InvestmentAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.LoanAccounts;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Mappers;

internal static partial class InfrastructureMapper
{
    public static AbstractAccountBase? Map(AccountDocument? document)
    {
        if (document == null)
            return default;

        var account = GetAccountFromDocument(document);

        return account;
    }

    public static IEnumerable<AbstractAccountBase> Map(IEnumerable<AccountDocument> documents)
    {
        return documents
            .Select(Map)
            .Where(x => x is not null)
            .Cast<AbstractAccountBase>();
    }

    public static AccountDocument? Map(AbstractAccountBase? account)
    {
        if (account is null)
            return default;

        if (account.Id.Equals(AccountId.Empty))
            throw new InvalidOperationException("Active Accounts must have an Id");

        if (account is CashAccount cash)
            return (AccountDocument)cash;

        if (account is CheckingAccount checking)
            return (AccountDocument)checking;

        if (account is SavingsAccount savings)
            return (AccountDocument)savings;

        if (account is CreditLineAccount creditLine)
            return (AccountDocument)creditLine;

        if (account is CreditCardAccount credCard)
            return (AccountDocument)credCard;

        if (account is LoanAccount loan)
            return (AccountDocument)loan;

        throw new InvalidOperationException("Account Type not supported");


        //if (account is CashAccount cashAccount)
        //{
        //    var cashDocument = new AccountDocument(
        //        cashAccount.Id,
        //        cashAccount.UserId,
        //        AccountEnums.CurrencyKeys.CASH,
        //        cashAccount.Name,
        //        cashAccount.IsCredit,
        //        cashAccount.Balance,
        //        cashAccount.Description,
        //        cashAccount.DisplayColor,
        //        cashAccount.DateCreated,
        //        cashAccount.DateUpdated,
        //        cashAccount.DateDeleted
        //    );

        //    return cashDocument;
        //}

        //if (account is AbstractBankAccount bankAccount)
        //{
        //    if (bankAccount is CheckingAccount checkingAccount)
        //    {
        //        var checkingDocument = new AccountDocument(
        //            checkingAccount.Id,
        //            checkingAccount.UserId,
        //            AccountEnums.CurrencyKeys.BANK,
        //            checkingAccount.Name,
        //            checkingAccount.IsCredit,
        //            checkingAccount.Balance,
        //            checkingAccount.Description,
        //            checkingAccount.DisplayColor,
        //            checkingAccount.DateCreated,
        //            checkingAccount.DateUpdated,
        //            checkingAccount.DateDeleted)
        //        {
        //            ExtAcctId = checkingAccount.ExtAcctId,
        //            AcctKey = checkingAccount.AcctKey,

        //            BankId = checkingAccount.BankId,
        //            BankName = checkingAccount.BankName,
        //            BranchId = checkingAccount.BranchId,

        //            BankAccountType = BankAccountEnums.CurrencyKeys.CHECKING,
        //            OverdraftLimit = checkingAccount.OverdraftLimit
        //        };

        //        return checkingDocument;
        //    }

        //    if (bankAccount is SavingsAccount savingsAccount)
        //    {
        //        var savingsDocument = new AccountDocument(
        //            savingsAccount.Id,
        //            savingsAccount.UserId,
        //            AccountEnums.CurrencyKeys.BANK,
        //            savingsAccount.Name,
        //            savingsAccount.IsCredit,
        //            savingsAccount.Balance,
        //            savingsAccount.Description,
        //            savingsAccount.DisplayColor,
        //            savingsAccount.DateCreated,
        //            savingsAccount.DateUpdated,
        //            savingsAccount.DateDeleted)
        //        {
        //            ExtAcctId = savingsAccount.ExtAcctId,
        //            AcctKey = savingsAccount.AcctKey,

        //            BankId = savingsAccount.BankId,
        //            BankName = savingsAccount.BankName,
        //            BranchId = savingsAccount.BranchId,

        //            BankAccountType = BankAccountEnums.CurrencyKeys.SAVINGS,

        //            InterestRate = savingsAccount.InterestRate
        //        };

        //        return savingsDocument;
        //    }

        //    if (bankAccount is CreditLineAccount creditLineAccount)
        //    {
        //        var creditLineDocument = new AccountDocument(
        //            creditLineAccount.Id,
        //            creditLineAccount.UserId,
        //            AccountEnums.CurrencyKeys.BANK,
        //            creditLineAccount.Name,
        //            creditLineAccount.IsCredit,
        //            creditLineAccount.Balance,
        //            creditLineAccount.Description,
        //            creditLineAccount.DisplayColor,
        //            creditLineAccount.DateCreated,
        //            creditLineAccount.DateUpdated,
        //            creditLineAccount.DateDeleted)
        //        {
        //            ExtAcctId = creditLineAccount.ExtAcctId,
        //            AcctKey = creditLineAccount.AcctKey,

        //            BankId = creditLineAccount.BankId,
        //            BankName = creditLineAccount.BankName,
        //            BranchId = creditLineAccount.BranchId,

        //            BankAccountType = BankAccountEnums.CurrencyKeys.CREDITLINE,

        //            CreditLimit = creditLineAccount.CreditLimit,
        //            InterestRate = creditLineAccount.InterestRate
        //        };

        //        return creditLineDocument;
        //    }
        //}

        //if (account is CreditCardAccount creditCardAccount)
        //{
        //    var creditCardDocument = new AccountDocument(
        //        creditCardAccount.Id,
        //        creditCardAccount.UserId,
        //        AccountEnums.CurrencyKeys.CREDITCARD,
        //        creditCardAccount.Name,
        //        creditCardAccount.IsCredit,
        //        creditCardAccount.Balance,
        //        creditCardAccount.Description,
        //        creditCardAccount.DisplayColor,
        //        creditCardAccount.DateCreated,
        //        creditCardAccount.DateUpdated,
        //        creditCardAccount.DateDeleted)
        //    {
        //        ExtAcctId = creditCardAccount.ExtAcctId,
        //        AcctKey = creditCardAccount.ACCTKEY,

        //        CreditLimit = creditCardAccount.CreditLimit,
        //        InterestRate = creditCardAccount.InterestRate
        //    };

        //    return creditCardDocument;
        //}

        //if (account is LoanAccount loanAccount)
        //{
        //    var loanAccountDocument = new AccountDocument(
        //        loanAccount.Id,
        //        loanAccount.UserId,
        //        AccountEnums.CurrencyKeys.LOAN,
        //        loanAccount.Name,
        //        loanAccount.IsCredit,
        //        loanAccount.Balance,
        //        loanAccount.Description,
        //        loanAccount.DisplayColor,
        //        loanAccount.DateCreated,
        //        loanAccount.DateUpdated,
        //        loanAccount.DateDeleted)
        //    {
        //        LoanAccountType = loanAccount.LoanAcctType,
        //        CreditLimit = loanAccount.CreditLimit,
        //        InterestRate = loanAccount.InterestRate
        //    };

        //    return loanAccountDocument;
        //}

        //throw new InvalidOperationException("BaseAccount Type not supported");
    }

    public static IEnumerable<AccountDocument> Map(IEnumerable<AbstractAccountBase> accounts)
    {
        return accounts.Select(a =>
            Map(a))
            .Where(x => x is not null)
            .Cast<AccountDocument>();
    }

    private static AbstractAccountBase? GetAccountFromDocument(AccountDocument? document)
    {
        if (document is null)
            return default;

        if (document.AccountType.Equals(AccountEnums.AccountKeys.CASH))
            return (CashAccount)document;

        if (document.AccountType.Equals(AccountEnums.AccountKeys.BANK))
        {
            if (document.BankAccountType.Equals(BankAccountEnums.BankAccountKeys.CHECKING))
                return (CheckingAccount)document;

            if (document.BankAccountType.Equals(BankAccountEnums.BankAccountKeys.SAVINGS))
                return (SavingsAccount)document;

            if (document.BankAccountType.Equals(BankAccountEnums.BankAccountKeys.CREDITLINE))
                return (CreditLineAccount)document;

            if (document.BankAccountType.Equals(BankAccountEnums.BankAccountKeys.CD))
            {
                throw new NotImplementedException("CD Account not implemented");
            }

            if (document.BankAccountType.Equals(BankAccountEnums.BankAccountKeys.MONEYMRKT))
            {
                throw new NotImplementedException("Money Market Account not implemented");
            }

            throw new InvalidOperationException($"Unknown Bank Account Type: {document.BankAccountType}");
        }

        if (document.AccountType.Equals(AccountEnums.AccountKeys.CREDITCARD))
        {
            return (CreditCardAccount)document;
        }

        if (document.AccountType.Equals(AccountEnums.AccountKeys.INVESTMENT))
        {
            return (InvestmentAccount)document;
        }

        if (document.AccountType.Equals(AccountEnums.AccountKeys.LOAN))
        {
            return (LoanAccount)document;
        }

        throw new InvalidOperationException("BaseAccount Type not supported");
    }
}