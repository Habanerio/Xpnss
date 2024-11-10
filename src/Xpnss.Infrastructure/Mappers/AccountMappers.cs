using Habanerio.Xpnss.Domain.Accounts;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.Documents;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Infrastructure.Mappers;

internal static partial class Mapper
{
    public static Account? Map(AccountDocument? document)
    {
        if (document == null)
            return null;

        var account = GetAccountFromDocument(document);

        return account;
    }

    public static IEnumerable<Account> Map(IEnumerable<AccountDocument> documents)
    {
        return documents.Select(Map).Where(x => x is not null).Cast<Account>();
    }

    public static AccountDocument? Map(Account? account)
    {
        if (account is null)
            return null;

        var accountId = ObjectId.Parse(account.Id);

        if (account is CashAccount)
        {
            var cashDocument = new CashAccountDocument(accountId)
            {
                UserId = account.UserId,
                AccountType = account.AccountType,
                Name = account.Name,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                Balance = account.Balance,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated
            };

            return cashDocument;
        }

        if (account is CheckingAccount checkingAccount)
        {
            var checkingDocument = new CheckingAccountDocument(accountId, checkingAccount.OverDraftAmount)
            {
                UserId = account.UserId,
                AccountType = account.AccountType,
                Name = account.Name,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                Balance = account.Balance,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated
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
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                Balance = account.Balance,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated
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
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                Balance = account.Balance,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated
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
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                Balance = account.Balance,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated
            };

            return lineOfCreditDocument;
        }

        throw new InvalidOperationException("Account Type not supported");
    }

    public static IEnumerable<AccountDocument> Map(IEnumerable<Account> accounts)
    {
        return accounts.Select(Map).Where(x => x is not null).Cast<AccountDocument>();
    }

    private static Account? GetAccountFromDocument(AccountDocument? document)
    {
        if (document is null)
            return null;

        if (document.AccountType.Equals(AccountTypes.Keys.Cash) && document is CashAccountDocument cashDoc)
        {
            var cashAccount = CashAccount.Load(
                new AccountId(cashDoc.Id.ToString()),

                new UserId(cashDoc.UserId),
                new AccountName(cashDoc.Name),
                new Money(cashDoc.Balance),
                cashDoc.Description,
                cashDoc.DisplayColor,

                cashDoc.DateCreated,
                cashDoc.DateClosed,
                cashDoc.DateDeleted,
                cashDoc.DateUpdated
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
                new Money(checkingDoc.OverDraftAmount),

                checkingDoc.DateCreated,
                checkingDoc.DateClosed,
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

                savingsDoc.DateCreated,
                savingsDoc.DateClosed,
                savingsDoc.DateDeleted,
                savingsDoc.DateUpdated
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

                ccDoc.DateCreated,
                ccDoc.DateClosed,
                ccDoc.DateDeleted,
                ccDoc.DateUpdated
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

                locDoc.DateCreated,
                locDoc.DateClosed,
                locDoc.DateDeleted,
                locDoc.DateUpdated
            );

            return lineOfCreditAccount;
        }

        throw new InvalidOperationException("Account Type not supported");
    }
}