using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Mappers;

internal static partial class InfrastructureMapper
{
    public static BaseAccount? Map(AccountDocument? document, bool includeTotals = false)
    {
        if (document == null)
            return default;

        var account = GetAccountFromDocument(document, includeTotals);

        return account;
    }

    public static IEnumerable<BaseAccount> Map(IEnumerable<AccountDocument> documents)
    {
        return documents
            .Select(d =>
                Map(d, false))
            .Where(x => x is not null)
            .Cast<BaseAccount>();
    }

    public static AccountDocument? Map(BaseAccount? account, bool includeTotals = false)
    {
        if (account is null)
            return default;

        var accountId = account.Id;

        if (account.EntityState == EntityState.ACTIVE && account.Id.Equals(AccountId.Empty))
            throw new InvalidOperationException("Active Accounts must have an Id");

        if (account is CashAccount)
        {
            var cashDocument = new CashAccountDocument(accountId)
            {
                UserId = account.UserId,
                AccountType = account.AccountType,
                Name = account.Name,
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated
            };

            return cashDocument;
        }

        if (account is CheckingAccount checkingAccount)
        {
            var checkingDocument = new CheckingAccountDocument(accountId, checkingAccount.OverdraftAmount)
            {
                UserId = account.UserId,
                AccountType = account.AccountType,
                Name = account.Name,
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated,
            };

            return checkingDocument;
        }

        if (account is SavingsAccount savingsAccount)
        {
            var savingsDocument = new SavingsAccountDocument(accountId, savingsAccount.InterestRate)
            {
                UserId = account.UserId,
                AccountType = account.AccountType,
                Name = account.Name,
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated,
            };

            return savingsDocument;
        }

        if (account is CreditCardAccount creditCardAccount)
        {
            var creditCardDocument = new CreditCardAccountDocument(accountId, creditCardAccount.CreditLimit, creditCardAccount.InterestRate)
            {
                UserId = account.UserId,
                AccountType = account.AccountType,
                Name = account.Name,
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated,
            };

            return creditCardDocument;
        }

        if (account is LineOfCreditAccount lineOfCreditAccount)
        {
            var lineOfCreditDocument = new LineOfCreditAccountDocument(accountId, lineOfCreditAccount.CreditLimit, lineOfCreditAccount.InterestRate)
            {
                UserId = account.UserId,
                AccountType = account.AccountType,
                Name = account.Name,
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated,
            };

            return lineOfCreditDocument;
        }

        throw new InvalidOperationException("BaseAccount Type not supported");
    }

    public static IEnumerable<AccountDocument> Map(IEnumerable<BaseAccount> accounts)
    {
        return accounts.Select(a => Map(a, false)).Where(x => x is not null).Cast<AccountDocument>();
    }

    private static BaseAccount? GetAccountFromDocument(AccountDocument? document, bool includeTotals = false)
    {
        if (document is null)
            return default;

        if (document.AccountType.Equals(AccountTypes.Keys.Cash) && document is CashAccountDocument cashDoc)
        {
            var cashAccount = CashAccount.Load(
                new AccountId(cashDoc.Id.ToString()),

                new UserId(cashDoc.UserId),
                new AccountName(cashDoc.Name),
                new Money(cashDoc.Balance),
                cashDoc.Description,
                cashDoc.DisplayColor,
                cashDoc.DateClosed,
                cashDoc.DateCreated,
                cashDoc.DateUpdated,
                cashDoc.DateDeleted
            );

            return cashAccount;
        }

        if (document.AccountType.Equals(AccountTypes.Keys.Checking) && document is CheckingAccountDocument checkingDoc)
        {
            var checkingAccount = CheckingAccount.Load(
                new AccountId(checkingDoc.Id.ToString()),

                new UserId(checkingDoc.UserId),
                new AccountName(checkingDoc.Name),
                new Money(checkingDoc.Balance),
                checkingDoc.Description,
                checkingDoc.DisplayColor,
                new Money(checkingDoc.OverdraftAmount),
                checkingDoc.DateClosed,
                checkingDoc.DateCreated,
                checkingDoc.DateDeleted,
                checkingDoc.DateUpdated
            );

            return checkingAccount;
        }

        if (document.AccountType.Equals(AccountTypes.Keys.Savings) && document is SavingsAccountDocument savingsDoc)
        {
            var savingsAccount = SavingsAccount.Load(
                new AccountId(savingsDoc.Id.ToString()),

                new UserId(savingsDoc.UserId),
                new AccountName(savingsDoc.Name),
                new Money(savingsDoc.Balance),
                savingsDoc.Description,
                savingsDoc.DisplayColor,
                new PercentageRate(savingsDoc.InterestRate),

                savingsDoc.DateClosed,
                savingsDoc.DateCreated,
                savingsDoc.DateUpdated,
                savingsDoc.DateDeleted
            );

            return savingsAccount;
        }

        if (document.AccountType.Equals(AccountTypes.Keys.CreditCard) && document is CreditCardAccountDocument ccDoc)
        {
            var creditCardAccount = CreditCardAccount.Load(
                new AccountId(ccDoc.Id.ToString()),

                new UserId(ccDoc.UserId),
                new AccountName(ccDoc.Name),
                new Money(ccDoc.Balance),
                ccDoc.Description,
                ccDoc.DisplayColor,
                new Money(ccDoc.CreditLimit),
                new PercentageRate(ccDoc.InterestRate),

                ccDoc.DateClosed,
                ccDoc.DateCreated,
                ccDoc.DateUpdated,
                ccDoc.DateDeleted
            );

            return creditCardAccount;
        }

        if (document.AccountType.Equals(AccountTypes.Keys.LineOfCredit) && document is LineOfCreditAccountDocument locDoc)
        {
            var lineOfCreditAccount = LineOfCreditAccount.Load(
                new AccountId(locDoc.Id.ToString()),

                new UserId(locDoc.UserId),
                new AccountName(locDoc.Name),
                new Money(locDoc.Balance),
                locDoc.Description,
                locDoc.DisplayColor,
                new Money(locDoc.CreditLimit),
                new PercentageRate(locDoc.InterestRate),

                locDoc.DateClosed,
                locDoc.DateCreated,
                locDoc.DateUpdated,
                locDoc.DateDeleted
            );

            return lineOfCreditAccount;
        }

        throw new InvalidOperationException("BaseAccount Type not supported");
    }
}