using Habanerio.Xpnss.Application.Accounts.DTOs;
using Habanerio.Xpnss.Domain.Accounts;

namespace Habanerio.Xpnss.Application.Mappers;

internal static partial class Mapper
{
    public static AccountDto? Map(Account? account)
    {
        if (account is null)
            return null;

        var accountDto = PopulateCommonDtoProperties(account);

        if (account.AccountType == AccountTypes.Keys.Cash)
        {
            if (account is not CashAccount)
                throw new InvalidOperationException("Account Type not supported");

            return accountDto;
        }

        if (account.AccountType == AccountTypes.Keys.Checking)
        {
            if (account is not CheckingAccount checkingAccount)
                throw new InvalidOperationException("Account Type not supported");

            accountDto.OverDraftAmount = checkingAccount.OverDraftAmount;

            return accountDto;
        }

        if (account.AccountType == AccountTypes.Keys.Savings)
        {
            if (account is not SavingsAccount savingsAccount)
                throw new InvalidOperationException("Account Type not supported");

            accountDto.InterestRate = savingsAccount.InterestRate.Value;

            return accountDto;
        }

        if (account.AccountType == AccountTypes.Keys.CreditCard)
        {
            if (account is not CreditCardAccount creditCardAccount)
                throw new InvalidOperationException("Account Type not supported");

            accountDto.CreditLimit = creditCardAccount.CreditLimit;
            accountDto.InterestRate = creditCardAccount.InterestRate.Value;

            return accountDto;
        }

        if (account.AccountType == AccountTypes.Keys.LineOfCredit)
        {
            if (account is not LineOfCreditAccount lineOfCreditAccount)
                throw new InvalidOperationException("Account Type not supported");

            accountDto.CreditLimit = lineOfCreditAccount.CreditLimit;
            accountDto.InterestRate = lineOfCreditAccount.InterestRate.Value;

            return accountDto;
        }

        return null;
    }

    public static IEnumerable<AccountDto> Map(IEnumerable<Account> accounts)
    {
        return accounts.Select(Map).Where(x => x is not null).Cast<AccountDto>();
    }

    private static AccountDto PopulateCommonDtoProperties(Account account)
    {
        var accountDto = new AccountDto()
        {
            Id = account.Id,
            UserId = account.UserId,
            AccountType = account.AccountType.ToString(),
            Name = account.Name,
            Balance = account.Balance,
            Description = account.Description,
            DisplayColor = account.DisplayColor,
            IsCredit = account.IsCredit,
            DateCreated = account.DateCreated,
            DateUpdated = account.DateUpdated,
            DateClosed = account.DateClosed,
            DateDeleted = account.DateDeleted
        };

        return accountDto;
    }
}