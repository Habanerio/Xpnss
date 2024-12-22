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

        if (account is InvestmentAccount investment)
            return (AccountDocument)investment;

        if (account is LoanAccount loan)
            return (AccountDocument)loan;

        throw new InvalidOperationException("Account Type not supported");
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