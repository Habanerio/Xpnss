using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Mappers;

internal static partial class InfrastructureMapper
{
    public static BaseAccount? Map(AccountDocument? document, bool includeTotals = false)
    {
        if (document == null)
            return null;

        var account = GetAccountFromDocument(document, includeTotals);

        return account;
    }

    public static IEnumerable<BaseAccount> Map(IEnumerable<AccountDocument> documents)
    {
        return documents.Select(d => Map(d, false)).Where(x => x is not null).Cast<BaseAccount>();
    }

    public static AccountDocument? Map(BaseAccount? account, bool includeTotals = false)
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
                //AdjustmentHistories = Map(account.AdjustmentHistories).ToList(),
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated
                //MonthlyTotals = Map(account.MonthlyTotals.ToList())
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
                //AdjustmentHistories = Map(account.AdjustmentHistories).ToList(),
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated,
                //MonthlyTotals = Map(account.MonthlyTotals.ToList())
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
                //AdjustmentHistories = Map(account.AdjustmentHistories).ToList(),
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated,
                //MonthlyTotals = Map(account.MonthlyTotals.ToList())
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
                //AdjustmentHistories = Map(account.AdjustmentHistories).ToList(),
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated,
                //MonthlyTotals = Map(account.MonthlyTotals.ToList())
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
                //AdjustmentHistories = Map(account.AdjustmentHistories).ToList(),
                Balance = account.Balance,
                Description = account.Description,
                DisplayColor = account.DisplayColor,
                DateCreated = account.DateCreated,
                DateClosed = account.DateClosed,
                DateDeleted = account.DateDeleted,
                DateUpdated = account.DateUpdated,
                //MonthlyTotals = Map(account.MonthlyTotals.ToList())
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

            //cashAccount.LoadMonthlyTotals(Map(cashDoc.MonthlyTotals ?? []).ToList());

            //cashAccount.LoadAdjustmentHistories(Map(cashDoc.AdjustmentHistories ?? []).ToList());


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
                checkingDoc.DateCreated,
                checkingDoc.DateClosed,
                checkingDoc.DateDeleted,
                checkingDoc.DateUpdated
            );

            if (includeTotals)
            {
                //checkingAccount.LoadMonthlyTotals(Map(checkingDoc.MonthlyTotals ?? []));

                //checkingAccount.LoadAdjustmentHistories(Map(checkingDoc.AdjustmentHistories ?? []).ToList());
            }

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

            if (includeTotals)
            {
                //savingsAccount.LoadMonthlyTotals(Map(savingsDoc.MonthlyTotals ?? []));

                //savingsAccount.LoadAdjustmentHistories(Map(savingsDoc.AdjustmentHistories ?? []).ToList());
            }

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

            if (includeTotals)
            {
                //creditCardAccount.LoadMonthlyTotals(Map(ccDoc.MonthlyTotals ?? []));

                //creditCardAccount.LoadAdjustmentHistories(Map(ccDoc.AdjustmentHistories ?? []).ToList());
            }

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

            if (includeTotals)
            {
                //lineOfCreditAccount.LoadMonthlyTotals(Map(locDoc.MonthlyTotals ?? []));

                //lineOfCreditAccount.LoadAdjustmentHistories(Map(locDoc.AdjustmentHistories ?? []).ToList());
            }

            return lineOfCreditAccount;
        }

        throw new InvalidOperationException("BaseAccount Type not supported");
    }
}