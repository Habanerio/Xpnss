using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Accounts.Application.Mappers;

internal static partial class ApplicationMapper
{
    public static AccountDto? Map(BaseAccount? entity, IEnumerable<MonthlyTotalDto>? monthlyTotals = null)
    {
        if (entity is null)
            return null;

        var accountDto = PopulateCommonDtoProperties(entity, monthlyTotals);

        if (entity.AccountType == AccountTypes.Keys.Cash)
        {
            if (entity is not CashAccount)
                throw new InvalidOperationException("Account Type not supported");

            return accountDto;
        }

        if (entity.AccountType == AccountTypes.Keys.Checking)
        {
            if (entity is not CheckingAccount checkingAccount)
                throw new InvalidOperationException("Account Type not supported");

            accountDto.OverdraftAmount = checkingAccount.OverdraftAmount;

            return accountDto;
        }

        if (entity.AccountType == AccountTypes.Keys.Savings)
        {
            if (entity is not SavingsAccount savingsAccount)
                throw new InvalidOperationException("Account Type not supported");

            accountDto.InterestRate = savingsAccount.InterestRate.Value;

            return accountDto;
        }

        if (entity.AccountType == AccountTypes.Keys.CreditCard)
        {
            if (entity is not CreditCardAccount creditCardAccount)
                throw new InvalidOperationException("Account Type not supported");

            accountDto.CreditLimit = creditCardAccount.CreditLimit;
            accountDto.InterestRate = creditCardAccount.InterestRate.Value;

            return accountDto;
        }

        if (entity.AccountType == AccountTypes.Keys.LineOfCredit)
        {
            if (entity is not LineOfCreditAccount lineOfCreditAccount)
                throw new InvalidOperationException("Account Type not supported");

            accountDto.CreditLimit = lineOfCreditAccount.CreditLimit;
            accountDto.InterestRate = lineOfCreditAccount.InterestRate.Value;

            return accountDto;
        }

        return null;
    }

    public static IEnumerable<AccountDto> Map(IEnumerable<BaseAccount> entities)
    {
        if (!entities.TryGetNonEnumeratedCount(out var count) || count == 0)
            return Enumerable.Empty<AccountDto>();

        return entities.Select(a => Map(a, null)).Where(x => x is not null).Cast<AccountDto>();
    }

    private static AccountDto PopulateCommonDtoProperties(BaseAccount entity, IEnumerable<MonthlyTotalDto>? monthlyTotals = null)
    {
        var accountDto = new AccountDto()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            AccountType = entity.AccountType.ToString(),
            Name = entity.Name,
            Balance = entity.Balance,
            Description = entity.Description,
            DisplayColor = entity.DisplayColor,
            IsCredit = entity.IsCredit,
            MonthlyTotals = monthlyTotals ?? [],
            DateCreated = entity.DateCreated,
            DateUpdated = entity.DateUpdated,
            DateClosed = entity.DateClosed,
            DateDeleted = entity.DateDeleted
        };

        return accountDto;
    }
}