using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.Requests;

public record CreateAccountApiRequest : UserRequiredApiRequest
{

    [JsonPropertyName("AccountType")]
    [JsonConverter(typeof(JsonNumberEnumConverter<AccountEnums.AccountKeys>))]
    public AccountEnums.AccountKeys AccountType { get; set; } =
        AccountEnums.AccountKeys.UNKNOWN;

    [JsonPropertyName("BankAccountType")]
    [JsonConverter(typeof(JsonNumberEnumConverter<BankAccountEnums.BankAccountKeys>))]
    public BankAccountEnums.BankAccountKeys BankAccountType { get; set; } =
        BankAccountEnums.BankAccountKeys.NA;

    [JsonPropertyName("InvestmentAccountType")]
    [JsonConverter(typeof(JsonNumberEnumConverter<InvestmentAccountEnums.InvestmentAccountKeys>))]
    public InvestmentAccountEnums.InvestmentAccountKeys InvestmentAccountType { get; set; } =
        InvestmentAccountEnums.InvestmentAccountKeys.NA;

    [JsonPropertyName("LoanAccountType")]
    [JsonConverter(typeof(JsonNumberEnumConverter<LoanAccountEnums.LoanAccountKeys>))]
    public LoanAccountEnums.LoanAccountKeys LoanAccountType { get; set; } =
        LoanAccountEnums.LoanAccountKeys.NA;


    public string Name { get; set; }

    public string Description { get; set; }

    public string DisplayColor { get; set; }


    public decimal CreditLimit { get; set; }

    public decimal InterestRate { get; set; }

    public bool IsDefault { get; set; }

    public decimal OverdraftAmount { get; set; }

    [JsonConstructor]
    protected CreateAccountApiRequest() { }

    public CreateAccountApiRequest(
        string userId,
        AllAccountEnums.AllAccountKeys accountType,
        string name,
        string description,
        string displayColor,
        bool isDefault = false) :
        this(
            userId,
            AllAccountEnums.GetTypes(accountType).AccountType,
            AllAccountEnums.GetTypes(accountType).BankType,
            AllAccountEnums.GetTypes(accountType).InvestmentType,
            AllAccountEnums.GetTypes(accountType).LoanType,
            name,
            description,
            displayColor,
            isDefault)
    { }

    protected CreateAccountApiRequest(
        string userId,
        AccountEnums.AccountKeys accountType,
        BankAccountEnums.BankAccountKeys bankAccountType,
        InvestmentAccountEnums.InvestmentAccountKeys investmentAccountType,
        LoanAccountEnums.LoanAccountKeys loanAccountType,
        string name,
        string description,
        string displayColor,
        bool isDefault = false)
    {
        UserId = userId;

        AccountType = accountType;
        BankAccountType = bankAccountType;
        InvestmentAccountType = investmentAccountType;
        LoanAccountType = loanAccountType;

        Name = name;
        Description = description;
        DisplayColor = displayColor;
        IsDefault = isDefault;
    }
}

public record CreateCashAccountRequest : CreateAccountApiRequest
{
    [JsonConstructor]
    public CreateCashAccountRequest()
    {
        AccountType = AccountEnums.AccountKeys.CASH;
        BankAccountType = BankAccountEnums.BankAccountKeys.NA;
        InvestmentAccountType = InvestmentAccountEnums.InvestmentAccountKeys.NA;
        LoanAccountType = LoanAccountEnums.LoanAccountKeys.NA;
    }

    public CreateCashAccountRequest(
        string userId,
        string name,
        string description,
        string displayColor,
        bool isDefault = false) :
        base(userId,
            AccountEnums.AccountKeys.CASH,
            BankAccountEnums.BankAccountKeys.NA,
            InvestmentAccountEnums.InvestmentAccountKeys.NA,
            LoanAccountEnums.LoanAccountKeys.NA,
            name, description, displayColor, isDefault)
    { }
}

public abstract record CreateBankAccountRequest : CreateAccountApiRequest
{
    [JsonConstructor]
    protected CreateBankAccountRequest()
    {
        AccountType = AccountEnums.AccountKeys.BANK;
        BankAccountType = BankAccountEnums.BankAccountKeys.NA;
        InvestmentAccountType = InvestmentAccountEnums.InvestmentAccountKeys.NA;
        LoanAccountType = LoanAccountEnums.LoanAccountKeys.NA;
    }

    protected CreateBankAccountRequest(
        string userId,
        BankAccountEnums.BankAccountKeys bankAccountType,
        string name,
        string description,
        string displayColor,
        bool isDefault = false) :
        base(
            userId,
            AccountEnums.AccountKeys.BANK,
            bankAccountType,
            InvestmentAccountEnums.InvestmentAccountKeys.NA,
            LoanAccountEnums.LoanAccountKeys.NA,
            name, description, displayColor, isDefault)
    { }
}

public record CreateCheckingAccountRequest : CreateBankAccountRequest
{
    [JsonConstructor]
    public CreateCheckingAccountRequest()
    {
        BankAccountType = BankAccountEnums.BankAccountKeys.CHECKING;
    }

    public CreateCheckingAccountRequest(
        string userId,
        string name,
        string description,
        string displayColor,
        decimal overdraft,
        bool isDefault = false) :
        base(
            userId,
            BankAccountEnums.BankAccountKeys.CHECKING,
            name, description, displayColor, isDefault)
    {
        OverdraftAmount = overdraft;
    }
}

public record CreateSavingsAccountRequest : CreateBankAccountRequest
{
    [JsonConstructor]
    public CreateSavingsAccountRequest()
    {
        BankAccountType = BankAccountEnums.BankAccountKeys.SAVINGS;
    }

    public CreateSavingsAccountRequest(
        string userId,
        string name,
        string description,
        string displayColor,
        decimal interestRate,
        bool isDefault = false) :
        base(
            userId,
            BankAccountEnums.BankAccountKeys.SAVINGS,
            name, description, displayColor, isDefault)
    {
        InterestRate = interestRate;
    }
}

public record CreateCreditLineAccountRequest : CreateBankAccountRequest
{
    [JsonConstructor]
    public CreateCreditLineAccountRequest()
    {
        BankAccountType = BankAccountEnums.BankAccountKeys.CREDITLINE;
    }

    public CreateCreditLineAccountRequest(
        string userId,
        string name,
        string description,
        string displayColor,
        decimal creditLimit,
        decimal interestRate,
        bool isDefault = false) :
        base(
            userId,
            BankAccountEnums.BankAccountKeys.CREDITLINE,
            name, description, displayColor, isDefault)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;
    }
}

public record CreateCreditCardAccountRequest : CreateAccountApiRequest
{
    [JsonConstructor]
    public CreateCreditCardAccountRequest()
    {
        AccountType = AccountEnums.AccountKeys.CREDITCARD;
        BankAccountType = BankAccountEnums.BankAccountKeys.NA;
        InvestmentAccountType = InvestmentAccountEnums.InvestmentAccountKeys.NA;
        LoanAccountType = LoanAccountEnums.LoanAccountKeys.NA;
    }

    public CreateCreditCardAccountRequest(
        string userId,
        string name,
        string description,
        string displayColor,
        decimal creditLimit,
        decimal interestRate,
        bool isDefault = false) :
        base(
            userId,
            AccountEnums.AccountKeys.CREDITCARD,
            BankAccountEnums.BankAccountKeys.NA,
            InvestmentAccountEnums.InvestmentAccountKeys.NA,
            LoanAccountEnums.LoanAccountKeys.NA,
            name, description, displayColor, isDefault)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;
    }
}

public record CreateLoanAccountRequest : CreateAccountApiRequest
{
    [JsonConstructor]
    public CreateLoanAccountRequest()
    {
        AccountType = AccountEnums.AccountKeys.LOAN;
        BankAccountType = BankAccountEnums.BankAccountKeys.NA;
        InvestmentAccountType = InvestmentAccountEnums.InvestmentAccountKeys.NA;
        LoanAccountType = LoanAccountEnums.LoanAccountKeys.NA;
    }

    public CreateLoanAccountRequest(
        string userId,
        string name,
        string description,
        string displayColor,
        LoanAccountEnums.LoanAccountKeys loanAccountType,
        decimal creditLimit,
        decimal interestRate,
        bool isDefault = false) :
        base(
            userId,
            AccountEnums.AccountKeys.LOAN,
            BankAccountEnums.BankAccountKeys.NA,
            InvestmentAccountEnums.InvestmentAccountKeys.NA,
            loanAccountType,
            name, description, displayColor, isDefault)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;

        LoanAccountType = loanAccountType;
    }
}