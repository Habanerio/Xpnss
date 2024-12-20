using Habanerio.Xpnss.Domain.Accounts;

namespace Habanerio.Xpnss.Application.Accounts.DTOs;

public record AccountDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string AccountType { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public decimal CreditLimit { get; set; }

    public string Description { get; set; }

    public string DisplayColor { get; set; }

    public decimal InterestRate { get; set; }

    public bool IsCredit { get; set; }

    public bool IsClosed => DateClosed.HasValue;

    public bool IsDeleted => DateDeleted.HasValue;

    //public List<MonthlyAccountTotal> MonthlyTotal { get; set; } = new();

    public decimal OverDraftAmount { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateClosed { get; set; }

    public DateTime? DateDeleted { get; set; }

    // Needed for deserialization in the API.
    //public AccountDto() { }
}

/*
public record CashAccountDto : AccountDto
{
    [JsonConstructor]
    public CashAccountDto() : base(AccountTypes.Keys.Cash, false) { }

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
        base(id, userId, AccountTypes.Keys.Cash, name, description, balance, displayColor, false, dateCreated, dateUpdated, dateDeleted)
    { }

    public static CashAccountDto New(
        string userId,
        string name,
        string description,
        decimal balance,
        string displayColor)
    {
        var accountDto = new CashAccountDto();

        New(accountDto, userId, name, description, balance, displayColor);

        return accountDto;
    }
}

public record CheckingAccountDto :
    AccountDto//, IHasOverdraftAmount
{
    public decimal OverDraftAmount { get; set; }

    //public bool IsOverDrafted { get; set; }

    [JsonConstructor]
    public CheckingAccountDto() : base(AccountTypes.Keys.Checking, false) { }

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
        base(id, userId, AccountTypes.Keys.Checking, name, description, balance, displayColor, false, dateCreated, dateUpdated, dateDeleted)
    {
        OverDraftAmount = overDraftAmount;
    }

    public static CheckingAccountDto New(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal overDraftAmount,
        string displayColor)
    {
        var accountDto = new CheckingAccountDto();

        New(accountDto, userId, name, description, balance, displayColor);

        accountDto.OverDraftAmount = overDraftAmount;

        return accountDto;
    }
}

public record SavingsAccountDto : AccountDto//, IHasInterestRate
{
    public decimal InterestRate { get; set; }

    [JsonConstructor]
    public SavingsAccountDto() : base(AccountTypes.Keys.Savings, false) { }

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
        base(id, userId, AccountTypes.Keys.Savings, name, description, balance, displayColor, false, dateCreated, dateUpdated, dateDeleted)
    {
        InterestRate = interestRate;
    }

    public static SavingsAccountDto New(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal interestRate,
        string displayColor)
    {
        var accountDto = new SavingsAccountDto();

        New(accountDto, userId, name, description, balance, displayColor);

        accountDto.InterestRate = interestRate;

        return accountDto;
    }
}

public abstract record CreditAccountDto : AccountDto//, IHasCreditLimit, IHasInterestRate
{
    public decimal CreditLimit { get; set; }

    public decimal InterestRate { get; set; }

    protected CreditAccountDto(AccountTypes.Keys accountTypes) : base(accountTypes, true) { }

    protected CreditAccountDto(
        string id,
        string userId,
        AccountTypes accountTypes,
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

    public static CreditAccountDto New(
        CreditAccountDto accountDtoDto,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor)
    {
        New(accountDtoDto, userId, name, description, balance, displayColor);

        accountDtoDto.CreditLimit = creditLimit;
        accountDtoDto.InterestRate = interestRate;

        return accountDtoDto;
    }
}

public record CreditCardAccountDto : CreditAccountDto
{
    [JsonConstructor]
    public CreditCardAccountDto() : base(AccountTypes.CreditCard) { }

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
        base(id, userId, AccountTypes.CreditCard, name, description, balance, creditLimit, interestRate, displayColor, dateCreated, dateUpdated, dateDeleted)
    { }

    public static CreditCardAccountDto New(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor)
    {
        var accountDto = new CreditCardAccountDto();

        New(accountDto, userId, name, description, balance, creditLimit, interestRate, displayColor);

        return accountDto;
    }
}

public record LineOfCreditAccountDto : CreditAccountDto
{
    [JsonConstructor]
    public LineOfCreditAccountDto() : base(AccountTypes.LineOfCredit) { }

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
        base(id, userId, AccountTypes.LineOfCredit, name, description, balance, creditLimit, interestRate, displayColor, dateCreated, dateUpdated, dateDeleted)
    { }

    public static LineOfCreditAccountDto New(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor)
    {
        var accountDto = new LineOfCreditAccountDto();

        New(accountDto, userId, name, description, balance, creditLimit, interestRate, displayColor);

        return accountDto;
    }
}
*/