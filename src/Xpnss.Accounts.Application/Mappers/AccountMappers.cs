using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CashAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CreditCardAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.InvestmentAccounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.LoanAccounts;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Accounts.Application.Mappers;

internal static partial class ApplicationMapper
{
    public static AccountDto? Map(AbstractAccountBase? entity)
    {
        if (entity is null)
            return default;

        var accountDto = PopulateCommonDtoProperties(entity);

        var accountType = entity.AccountType;

        if (accountType.Equals(AccountEnums.AccountKeys.CASH))
        {
            if (entity is not CashAccount)
                throw new InvalidOperationException(
                    $"Was expecting a {accountType} AccountType, but Account was {entity.GetType()}");

            accountDto.AccountType = AccountEnums.AccountKeys.CASH;
            accountDto.BankAccountType = BankAccountEnums.BankAccountKeys.NA;
            accountDto.InvestmentAccountType = InvestmentAccountEnums.InvestmentAccountKeys.NA;
            accountDto.LoanAccountType = LoanAccountEnums.LoanAccountKeys.NA;

            return accountDto;
        }

        if (entity.AccountType == AccountEnums.AccountKeys.BANK)
        {
            if (!(entity is AbstractBankAccount bankAccountEntity))
                throw new InvalidOperationException(
                    $"Was expecting a {accountType} AccountType, but Account was {entity.GetType()}");

            accountDto.AccountType = AccountEnums.AccountKeys.BANK;
            accountDto.BankAccountType = bankAccountEntity.BankAccountType;
            accountDto.InvestmentAccountType = InvestmentAccountEnums.InvestmentAccountKeys.NA;
            accountDto.LoanAccountType = LoanAccountEnums.LoanAccountKeys.NA;

            accountDto.ClosedDate = bankAccountEntity.ClosedDate;

            accountDto.ExtAcctId = bankAccountEntity.ExtAcctId;
            accountDto.InstitutionName = bankAccountEntity.BankName;

            if (bankAccountEntity is CheckingAccount checkingAccount)
            {
                if (!checkingAccount.BankAccountType.Equals(BankAccountEnums.BankAccountKeys.CHECKING))
                    throw new InvalidOperationException(
                        $"Was expecting a {accountType} AccountType, but Account was {bankAccountEntity.GetType()}");

                accountDto.IsOverLimit = checkingAccount.IsOverLimit;
                accountDto.OverdraftLimit = checkingAccount.OverdraftLimit;

                return accountDto;
            }

            if (bankAccountEntity is SavingsAccount savingsAccount)
            {
                if (!savingsAccount.BankAccountType.Equals(BankAccountEnums.BankAccountKeys.SAVINGS))
                    throw new InvalidOperationException(
                        $"Was expecting a {accountType} AccountType, but Account was {bankAccountEntity.GetType()}");

                accountDto.InterestRate = savingsAccount.InterestRate.Value;

                return accountDto;
            }

            if (bankAccountEntity is CreditLineAccount creditLineAccount)
            {
                if (!creditLineAccount.BankAccountType.Equals(BankAccountEnums.BankAccountKeys.CREDITLINE))
                    throw new InvalidOperationException(
                        $"Was expecting a {accountType} AccountType, but Account was {bankAccountEntity.GetType()}");

                accountDto.CreditLimit = creditLineAccount.CreditLimit;
                accountDto.InterestRate = creditLineAccount.InterestRate.Value;
                accountDto.IsOverLimit = creditLineAccount.IsOverLimit;

                return accountDto;
            }
        }

        if (entity.AccountType == AccountEnums.AccountKeys.CREDITCARD)
        {
            if (entity is not CreditCardAccount ccAccount)
                throw new InvalidOperationException(
                    $"Was expecting a {accountType} AccountType, but Account was {entity.GetType()}");

            accountDto.AccountType = AccountEnums.AccountKeys.CREDITCARD;
            accountDto.BankAccountType = BankAccountEnums.BankAccountKeys.NA;
            accountDto.InvestmentAccountType = InvestmentAccountEnums.InvestmentAccountKeys.NA;
            accountDto.LoanAccountType = LoanAccountEnums.LoanAccountKeys.NA;

            accountDto.ClosedDate = ccAccount.ClosedDate;

            accountDto.ExtAcctId = ccAccount.ExtAcctId;
            accountDto.InstitutionName = ccAccount.InstitutionName;

            accountDto.CreditLimit = ccAccount.CreditLimit;
            accountDto.InterestRate = ccAccount.InterestRate.Value;

            accountDto.IsOverLimit = ccAccount.IsOverLimit;

            return accountDto;
        }

        if (entity.AccountType == AccountEnums.AccountKeys.INVESTMENT)
        {
            if (entity is not InvestmentAccount investmentAccount)
                throw new InvalidOperationException(
                    $"Was expecting a {accountType} AccountType, but Account was {entity.GetType()}");

            accountDto.AccountType = AccountEnums.AccountKeys.LOAN;
            accountDto.BankAccountType = BankAccountEnums.BankAccountKeys.NA;
            accountDto.InvestmentAccountType = InvestmentAccountEnums.InvestmentAccountKeys.NA;
            accountDto.LoanAccountType = LoanAccountEnums.LoanAccountKeys.NA;

            accountDto.ClosedDate = investmentAccount.ClosedDate;

            accountDto.ExtAcctId = investmentAccount.ExtAcctId;
            accountDto.InstitutionName = investmentAccount.InstitutionName;
        }

        if (entity.AccountType == AccountEnums.AccountKeys.LOAN)
        {
            if (entity is not LoanAccount loanAccount)
                throw new InvalidOperationException("Account Type not supported");

            accountDto.AccountType = AccountEnums.AccountKeys.LOAN;
            accountDto.BankAccountType = BankAccountEnums.BankAccountKeys.NA;
            accountDto.InvestmentAccountType = InvestmentAccountEnums.InvestmentAccountKeys.NA;
            accountDto.LoanAccountType = LoanAccountEnums.LoanAccountKeys.NA;

            accountDto.ClosedDate = loanAccount.ClosedDate;

            accountDto.CreditLimit = loanAccount.CreditLimit;
            accountDto.InterestRate = loanAccount.InterestRate.Value;
            accountDto.IsOverLimit = loanAccount.IsOverLimit;

            return accountDto;
        }

        return default;
    }

    public static IEnumerable<AccountDto> Map(IEnumerable<AbstractAccountBase> entities)
    {
        var entitiesArray = entities?.ToArray() ?? [];

        if (!entitiesArray.Any())
            return Enumerable.Empty<AccountDto>();

        return entitiesArray.Select(a =>
            Map(a))
            .Where(x => x is not null)
            .Cast<AccountDto>();
    }

    private static AccountDto PopulateCommonDtoProperties(AbstractAccountBase entity)
    {
        var accountDto = new AccountDto()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            AccountType = entity.AccountType,
            Name = entity.Name,
            Balance = entity.Balance,
            Description = entity.Description,
            DisplayColor = entity.DisplayColor,
            IsCredit = entity.IsCredit,
            IsDefault = entity.IsDefault,
            SortOrder = entity.SortOrder,
            DateCreated = entity.DateCreated,
            DateUpdated = entity.DateUpdated,
            DateDeleted = entity.DateDeleted
        };

        return accountDto;
    }
}