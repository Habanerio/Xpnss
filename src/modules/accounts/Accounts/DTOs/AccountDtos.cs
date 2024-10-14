using System.Diagnostics.CodeAnalysis;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;

namespace Habanerio.Xpnss.Modules.Accounts.DTOs;

public record AccountDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string AccountType { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public string Description { get; set; }

    public string DisplayColor { get; set; }

    public bool IsCredit { get; set; }

    public bool IsDeleted => DateDeleted.HasValue;

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }

    // Needed for deserialization in the API.
    public AccountDto() { }

    protected AccountDto(AccountTypes accountTypes, bool isCredit)
    {
        AccountType = accountTypes.ToString();
        IsCredit = isCredit;
    }

    protected AccountDto(
        string id,
        string userId,
        AccountTypes accountTypes,
        string name,
        string description,
        decimal balance,
        /*[StringSyntax(StringSyntaxAttribute.Regex)] */
        string displayColor,
        bool isCredit,
        DateTimeOffset dateCreated,
        DateTimeOffset? dateUpdated = null,
        DateTimeOffset? dateDeleted = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("AccountId cannot be null or empty", nameof(id));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be null or empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        if (!displayColor.StartsWith('#') || displayColor.Length > 7)
            throw new ArgumentException("DisplayColor must be a valid hex color code", nameof(displayColor));

        Id = id;
        UserId = userId;
        AccountType = accountTypes.ToString();
        Name = name;
        Balance = balance;
        Description = description ?? string.Empty;
        DisplayColor = displayColor ?? string.Empty;
        IsCredit = isCredit;
        DateCreated = dateCreated.UtcDateTime;
        DateUpdated = dateUpdated?.UtcDateTime;
        DateDeleted = dateDeleted?.UtcDateTime;
    }

    internal AccountDto(
        AccountDto accountDto,
        string userId,
        string name,
        string description,
        decimal balance,
        string displayColor)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be null or empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        if (!displayColor.StartsWith('#') || displayColor.Length > 7)
            throw new ArgumentException("DisplayColor must be a valid hex color code", nameof(displayColor));

        accountDto.Id = string.Empty;
        accountDto.UserId = userId;
        accountDto.Name = name;
        accountDto.Balance = balance;
        accountDto.Description = description;
        accountDto.DisplayColor = displayColor;

        accountDto.DateCreated = DateTime.UtcNow;
    }

    internal static AccountDto New(
        AccountDto accountDto,
        string userId,
        string name,
        string description,
        decimal balance,
        string displayColor)
    {
        return new AccountDto(accountDto, userId, name, description, balance, displayColor);
    }
}


public record CashAccountDto : AccountDto
{
    private CashAccountDto() : base(AccountTypes.Cash, false) { }

    public CashAccountDto(
        string id,
        string userId,
        string name,
        string description,
        decimal balance,
        string displayColor,
        DateTimeOffset dateCreated,
        DateTimeOffset? dateUpdated = null,
        DateTimeOffset? dateDeleted = null) :
        base(id, userId, AccountTypes.Cash, name, description, balance, displayColor, false, dateCreated, dateUpdated, dateDeleted)
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
    AccountDto, IHasOverdraftAmount
{
    public decimal OverDraftAmount { get; set; }

    //public bool IsOverDrafted { get; set; }

    private CheckingAccountDto() : base(AccountTypes.Checking, false) { }

    public CheckingAccountDto(
        string id,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal overDraftAmount,
        string displayColor,
        DateTimeOffset dateCreated,
        DateTimeOffset? dateUpdated = null,
        DateTimeOffset? dateDeleted = null) :
        base(id, userId, AccountTypes.Checking, name, description, balance, displayColor, false, dateCreated, dateUpdated, dateDeleted)
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

public record SavingsAccountDto : AccountDto, IHasInterestRate
{
    public decimal InterestRate { get; set; }

    private SavingsAccountDto() : base(AccountTypes.Savings, false) { }

    public SavingsAccountDto(
        string id,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal interestRate,
        string displayColor,
        DateTimeOffset dateCreated,
        DateTimeOffset? dateUpdated = null,
        DateTimeOffset? dateDeleted = null) :
        base(id, userId, AccountTypes.Savings, name, description, balance, displayColor, false, dateCreated, dateUpdated, dateDeleted)
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

public abstract record CreditAccountDto : AccountDto, IHasCreditLimit, IHasInterestRate
{
    public decimal CreditLimit { get; set; }

    public decimal InterestRate { get; set; }

    protected CreditAccountDto(AccountTypes accountTypes) : base(accountTypes, true) { }

    public CreditAccountDto(
        string id,
        string userId,
        AccountTypes accountTypes,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor,
        DateTimeOffset dateCreated,
        DateTimeOffset? dateUpdated = null,
        DateTimeOffset? dateDeleted = null) :
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
    private CreditCardAccountDto() : base(AccountTypes.CreditCard) { }

    public CreditCardAccountDto(string id,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor,
        DateTimeOffset dateCreated,
        DateTimeOffset? dateUpdated = null,
        DateTimeOffset? dateDeleted = null) :
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
    private LineOfCreditAccountDto() : base(AccountTypes.LineOfCredit) { }

    public LineOfCreditAccountDto(
        string id,
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor,
        DateTimeOffset dateCreated,
        DateTimeOffset? dateUpdated = null,
        DateTimeOffset? dateDeleted = null) :
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