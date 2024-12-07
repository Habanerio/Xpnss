using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.DTOs;

public sealed record AccountDto
{
    #region - Common Properties -

    public string Id { get; set; }

    public string UserId { get; set; }

    public AccountEnums.AccountKeys AccountType { get; set; } =
        AccountEnums.AccountKeys.UNKNOWN;

    public BankAccountEnums.BankAccountKeys BankAccountType { get; set; } =
        BankAccountEnums.BankAccountKeys.NA;

    public LoanAccountEnums.LoanAccountKeys LoanAccountType { get; set; } =
        LoanAccountEnums.LoanAccountKeys.NA;

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public DateTime? ClosedDate { get; set; }

    public string Description { get; set; } = "";

    public string DisplayColor { get; set; } = "";

    public bool IsCredit { get; set; }

    #endregion

    #region - Bank Account Properties -

    public string ExtAcctId { get; set; } = "";

    public string InstitutionName { get; set; } = "";

    #endregion


    public decimal CreditLimit { get; set; }

    public decimal InterestRate { get; set; }

    public decimal OverdraftLimit { get; set; }


    public bool IsClosed => ClosedDate.HasValue;

    public bool IsDeleted => DateDeleted.HasValue;

    public bool IsOverLimit { get; set; }


    public IEnumerable<MonthlyTotalDto> MonthlyTotals { get; set; } = [];


    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }

    [JsonConstructor]
    public AccountDto() { }
}

public sealed record AccountItemDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string AccountType { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public string Description { get; set; }

    public string DisplayColor { get; set; }
}

/*
public sealed record CashAccountDto : AccountDto
{
    [JsonConstructor]
    public CashAccountDto() : base(AccountEnums.CurrencyKeys.Cash, false) { }

    public CashAccountDto(
        string id,
        string userId,
        string name,
        string description,
        decimal balance,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(id, userId, AccountEnums.CurrencyKeys.Cash, name, description, balance, displayColor, false, dateCreated, dateUpdated, dateDeleted)
    { }

    public static CashAccountDto NewId(
        string userId,
        string name,
        string description,
        decimal balance,
        string displayColor)
    {
        var accountDto = new CashAccountDto();

        NewId(accountDto, userId, name, description, balance, displayColor);

        return accountDto;
    }
}

public sealed record CheckingAccountDto :
    AccountDto//, IHasOverdraftAmount
{
    public decimal OverdraftLimit { get; set; }

    //public bool IsOverdrafted { get; set; }

    [JsonConstructor]
    public CheckingAccountDto() : base(AccountEnums.CurrencyKeys.Checking, false) { }

    public CheckingAccountDto(
        string id,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal overDraftAmount,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(id, userId, AccountEnums.CurrencyKeys.Checking, name, description, balance, displayColor, false, dateCreated, dateUpdated, dateDeleted)
    {
        OverdraftLimit = overDraftAmount;
    }

    public static CheckingAccountDto NewId(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal overDraftAmount,
        string displayColor)
    {
        var accountDto = new CheckingAccountDto();

        NewId(accountDto, userId, name, description, balance, displayColor);

        accountDto.OverdraftLimit = overDraftAmount;

        return accountDto;
    }
}

public sealed record SavingsAccountDto : AccountDto//, IHasInterestRate
{
    public decimal InterestRate { get; set; }

    [JsonConstructor]
    public SavingsAccountDto() : base(AccountEnums.CurrencyKeys.Savings, false) { }

    public SavingsAccountDto(
        string id,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal interestRate,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(id, userId, AccountEnums.CurrencyKeys.Savings, name, description, balance, displayColor, false, dateCreated, dateUpdated, dateDeleted)
    {
        InterestRate = interestRate;
    }

    public static SavingsAccountDto NewId(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal interestRate,
        string displayColor)
    {
        var accountDto = new SavingsAccountDto();

        NewId(accountDto, userId, name, description, balance, displayColor);

        accountDto.InterestRate = interestRate;

        return accountDto;
    }
}

public abstract record CreditAccountDto : AccountDto//, IHasCreditLimit, IHasInterestRate
{
    public decimal CreditLimit { get; set; }

    public decimal InterestRate { get; set; }

    protected CreditAccountDto(AccountEnums accountTypes) : base(accountTypes, true) { }

    protected CreditAccountDto(
        string id,
        string userId,
        AccountEnums accountTypes,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(id, userId, accountTypes, name, description, balance, displayColor, true, dateCreated, dateUpdated, dateDeleted)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;
    }

    public static CreditAccountDto NewId(
        CreditAccountDto accountDtoDto,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor)
    {
        NewId(accountDtoDto, userId, name, description, balance, displayColor);

        accountDtoDto.CreditLimit = creditLimit;
        accountDtoDto.InterestRate = interestRate;

        return accountDtoDto;
    }
}

public record CreditCardAccountDto : CreditAccountDto
{
    [JsonConstructor]
    public CreditCardAccountDto() : base(AccountEnums.CurrencyKeys.CreditCard) { }

    public CreditCardAccountDto(string id,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(id, userId, AccountEnums.CurrencyKeys.CreditCard, name, description, balance, creditLimit, interestRate, displayColor, dateCreated, dateUpdated, dateDeleted)
    { }

    public static CreditCardAccountDto NewId(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor)
    {
        var accountDto = new CreditCardAccountDto();

        NewId(accountDto, userId, name, description, balance, creditLimit, interestRate, displayColor);

        return accountDto;
    }
}

public record LineOfCreditAccountDto : CreditAccountDto
{
    [JsonConstructor]
    public LineOfCreditAccountDto() : base(AccountEnums.CurrencyKeys.LineOfCredit) { }

    public LineOfCreditAccountDto(
        string id,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(id, userId, AccountEnums.CurrencyKeys.LineOfCredit, name, description, balance, creditLimit, interestRate, displayColor, dateCreated, dateUpdated, dateDeleted)
    { }

    public static LineOfCreditAccountDto NewId(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor)
    {
        var accountDto = new LineOfCreditAccountDto();

        NewId(accountDto, userId, name, description, balance, creditLimit, interestRate, displayColor);

        return accountDto;
    }
}
*/