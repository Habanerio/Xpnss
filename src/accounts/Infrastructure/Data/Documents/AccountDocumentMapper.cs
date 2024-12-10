using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CashAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CreditCardAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.InvestmentAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.LoanAccounts;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;

public partial class AccountDocument
{
    public static explicit operator AccountDocument(CashAccount account)
    {
        return new AccountDocument()
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,

            AccountType = account.AccountType,
            BankAccountType = account.BankAccountType,
            LoanAccountType = account.LoanAccountType,

            ExtAcctId = string.Empty,
            InstitutionName = string.Empty,

            Balance = account.Balance,
            Description = account.Description,
            DisplayColor = account.DisplayColor,

            IsCredit = account.IsCredit,
            IsDefault = account.IsDefault,

            SortOrder = account.SortOrder,

            DateCreated = account.DateCreated,
            DateUpdated = account.DateUpdated,
            DateDeleted = account.DateDeleted
        };
    }

    public static explicit operator AccountDocument(CheckingAccount account)
    {
        return new AccountDocument()
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,

            AccountType = account.AccountType,
            BankAccountType = account.BankAccountType,
            LoanAccountType = account.LoanAccountType,

            ExtAcctId = account.ExtAcctId,
            InstitutionName = account.BankName,

            Balance = account.Balance,
            ClosedDate = account.ClosedDate,
            Description = account.Description,
            DisplayColor = account.DisplayColor,

            OverdraftLimit = account.OverdraftLimit,

            IsCredit = account.IsCredit,
            IsDefault = account.IsDefault,
            IsOverLimit = account.IsOverLimit,

            SortOrder = account.SortOrder,

            DateCreated = account.DateCreated,
            DateUpdated = account.DateUpdated,
            DateDeleted = account.DateDeleted
        };
    }

    public static explicit operator AccountDocument(SavingsAccount account)
    {
        return new AccountDocument()
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,

            AccountType = account.AccountType,
            BankAccountType = account.BankAccountType,
            LoanAccountType = account.LoanAccountType,

            ExtAcctId = account.ExtAcctId,
            InstitutionName = account.BankName,

            Balance = account.Balance,
            ClosedDate = account.ClosedDate,
            Description = account.Description,
            DisplayColor = account.DisplayColor,

            InterestRate = account.InterestRate,

            IsCredit = account.IsCredit,
            IsDefault = account.IsDefault,
            SortOrder = account.SortOrder,

            DateCreated = account.DateCreated,
            DateUpdated = account.DateUpdated,
            DateDeleted = account.DateDeleted
        };
    }

    public static explicit operator AccountDocument(CreditLineAccount account)
    {
        return new AccountDocument()
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,

            AccountType = account.AccountType,
            BankAccountType = account.BankAccountType,
            LoanAccountType = account.LoanAccountType,

            ExtAcctId = account.ExtAcctId,
            InstitutionName = account.BankName,

            Balance = account.Balance,
            ClosedDate = account.ClosedDate,
            Description = account.Description,
            DisplayColor = account.DisplayColor,

            CreditLimit = account.CreditLimit,
            InterestRate = account.InterestRate,

            IsCredit = account.IsCredit,
            IsDefault = account.IsDefault,
            IsOverLimit = account.IsOverLimit,

            SortOrder = account.SortOrder,

            DateCreated = account.DateCreated,
            DateUpdated = account.DateUpdated,
            DateDeleted = account.DateDeleted
        };
    }

    public static explicit operator AccountDocument(CreditCardAccount account)
    {
        return new AccountDocument()
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,

            AccountType = account.AccountType,
            BankAccountType = account.BankAccountType,
            LoanAccountType = account.LoanAccountType,

            ExtAcctId = account.ExtAcctId,
            InstitutionName = account.InstitutionName,

            Balance = account.Balance,
            ClosedDate = account.ClosedDate,
            Description = account.Description,
            DisplayColor = account.DisplayColor,

            CreditLimit = account.CreditLimit,
            InterestRate = account.InterestRate,

            IsCredit = account.IsCredit,
            IsDefault = account.IsDefault,
            IsOverLimit = account.IsOverLimit,

            SortOrder = account.SortOrder,

            DateCreated = account.DateCreated,
            DateUpdated = account.DateUpdated,
            DateDeleted = account.DateDeleted
        };
    }

    public static explicit operator AccountDocument(InvestmentAccount account)
    {
        return new AccountDocument()
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,

            AccountType = account.AccountType,
            BankAccountType = account.BankAccountType,
            LoanAccountType = account.LoanAccountType,

            ExtAcctId = account.ExtAcctId,
            InstitutionName = account.InstitutionName,

            Balance = account.Balance,
            ClosedDate = account.ClosedDate,
            Description = account.Description,
            DisplayColor = account.DisplayColor,

            IsCredit = account.IsCredit,
            IsDefault = account.IsDefault,

            SortOrder = account.SortOrder,

            DateCreated = account.DateCreated,
            DateUpdated = account.DateUpdated,
            DateDeleted = account.DateDeleted
        };
    }

    public static explicit operator AccountDocument(LoanAccount account)
    {
        return new AccountDocument()
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,

            AccountType = account.AccountType,
            BankAccountType = account.BankAccountType,
            LoanAccountType = account.LoanAccountType,

            ExtAcctId = account.ExtAcctId,
            InstitutionName = account.InstitutionName,

            Balance = account.Balance,
            ClosedDate = account.ClosedDate,
            Description = account.Description,
            DisplayColor = account.DisplayColor,

            CreditLimit = account.CreditLimit,
            InterestRate = account.InterestRate,

            IsCredit = account.IsCredit,
            IsDefault = account.IsDefault,
            IsOverLimit = account.IsOverLimit,

            SortOrder = account.SortOrder,

            DateCreated = account.DateCreated,
            DateUpdated = account.DateUpdated,
            DateDeleted = account.DateDeleted
        };
    }


    public static explicit operator CashAccount(AccountDocument document)
    {
        return CashAccount.Load(
            new AccountId(document.Id),
            new UserId(document.UserId),
            new AccountName(document.Name),
            new Money(document.Balance),
            document.Description,
            document.DisplayColor,
            document.IsDefault,
            document.SortOrder,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
    }

    public static explicit operator CheckingAccount(AccountDocument document)
    {
        return CheckingAccount.Load(
            new AccountId(document.Id),
            new UserId(document.UserId),
            new AccountName(document.Name),
            new Money(document.Balance),
            document.InstitutionName,
            document.ClosedDate,
            document.Description,
            document.DisplayColor,
            document.ExtAcctId,
            document.IsDefault,
            new Money(document.OverdraftLimit),
            document.SortOrder,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
    }

    public static explicit operator SavingsAccount(AccountDocument document)
    {
        return SavingsAccount.Load(
            new AccountId(document.Id),
            new UserId(document.UserId),
            new AccountName(document.Name),
            new Money(document.Balance),
            document.InstitutionName,
            document.ClosedDate,
            document.Description,
            document.DisplayColor,
            document.ExtAcctId,
            new PercentageRate(document.InterestRate),
            document.IsDefault,
            document.SortOrder,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
    }

    public static explicit operator CreditLineAccount(AccountDocument document)
    {
        return CreditLineAccount.Load(
            new AccountId(document.Id),
            new UserId(document.UserId),
            new AccountName(document.Name),
            new Money(document.Balance),
            document.InstitutionName,
            document.ClosedDate,
            document.Description,
            document.DisplayColor,
            document.ExtAcctId,
            new Money(document.CreditLimit),
            new PercentageRate(document.InterestRate),
            document.IsDefault,
            document.SortOrder,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
    }

    public static explicit operator CreditCardAccount(AccountDocument document)
    {
        return CreditCardAccount.Load(
            new AccountId(document.Id),
            new UserId(document.UserId),
            new AccountName(document.Name),
            new Money(document.Balance),
            document.ClosedDate,
            document.Description,
            document.DisplayColor,
            document.ExtAcctId,
            document.InstitutionName,
            new Money(document.CreditLimit),
            new PercentageRate(document.InterestRate),
            document.IsDefault,
            document.SortOrder,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
    }

    public static explicit operator InvestmentAccount(AccountDocument document)
    {
        return InvestmentAccount.Load(
            new AccountId(document.Id),
            new UserId(document.UserId),
            new AccountName(document.Name),
            new Money(document.Balance),
            document.ClosedDate,
            document.Description,
            document.DisplayColor,
            document.ExtAcctId,
            document.InstitutionName,
            document.IsDefault,
            document.SortOrder,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
    }

    public static explicit operator LoanAccount(AccountDocument document)
    {
        return LoanAccount.Load(
            new AccountId(document.Id),
            new UserId(document.UserId),
            document.LoanAccountType,
            new AccountName(document.Name),
            new Money(document.Balance),
            document.ClosedDate,
            document.Description,
            document.DisplayColor,
            document.InstitutionName,
            document.ExtAcctId,
            new Money(document.CreditLimit),
            new PercentageRate(document.InterestRate),
            document.IsDefault,
            document.SortOrder,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
    }

}