using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.Requests;

public record CreateAccountRequest
{
    public string UserId { get; set; }

    [JsonPropertyName("AccountType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountEnums.AccountKeys AccountType { get; set; } =
        AccountEnums.AccountKeys.UNKNOWN;

    [JsonPropertyName("BankAccountType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BankAccountEnums.BankAccountKeys BankAccountType { get; set; } =
        BankAccountEnums.BankAccountKeys.NA;

    [JsonPropertyName("InvestmentAccountType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InvestmentAccountEnums.InvestmentAccountKeys InvestmentAccountType { get; set; } =
        InvestmentAccountEnums.InvestmentAccountKeys.NA;

    [JsonPropertyName("LoanAccountType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LoanAccountEnums.LoanAccountKeys LoanAccountType { get; set; } =
        LoanAccountEnums.LoanAccountKeys.NA;

    public string Name { get; set; }

    public string Description { get; set; }

    public string DisplayColor { get; set; }


    public decimal CreditLimit { get; set; }

    public decimal InterestRate { get; set; }

    public decimal OverdraftAmount { get; set; }

    protected CreateAccountRequest() { }

    [JsonConstructor]
    public CreateAccountRequest(
        string userId,
        AllAccountEnums.Keys accountType,
        string name,
        string description,
        string displayColor) :
        this(
            userId,
            AllAccountEnums.GetTypes(accountType).AccountType,
            AllAccountEnums.GetTypes(accountType).BankType,
            AllAccountEnums.GetTypes(accountType).InvestmentType,
            AllAccountEnums.GetTypes(accountType).LoanType,
            name,
            description,
            displayColor)
    { }

    protected CreateAccountRequest(
        string userId,
        AccountEnums.AccountKeys accountType,
        BankAccountEnums.BankAccountKeys bankAccountType,
        InvestmentAccountEnums.InvestmentAccountKeys investmentAccountType,
        LoanAccountEnums.LoanAccountKeys loanAccountType,
        string name,
        string description,
        string displayColor)
    {
        AccountType = accountType;
        BankAccountType = bankAccountType;
        LoanAccountType = loanAccountType;

        Name = name;
        Description = description;
        DisplayColor = displayColor;
        UserId = userId;
    }
}

public record CreateCashAccountRequest : CreateAccountRequest
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
        string displayColor) :
        base(userId,
            AccountEnums.AccountKeys.CASH,
            BankAccountEnums.BankAccountKeys.NA,
            InvestmentAccountEnums.InvestmentAccountKeys.NA,
            LoanAccountEnums.LoanAccountKeys.NA,
            name, description, displayColor)
    { }

    //public static explicit operator CreateAccountCommand(CreateCashAccountRequest request)
    //{
    //    var command = new CreateAccountCommand(
    //        request.UserId,
    //        request.AccountType,
    //        request.Name,
    //        request.Description,
    //        request.DisplayColor);


    //    return command;
    //}
}

public abstract record CreateBankAccountRequest : CreateAccountRequest
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
        string displayColor) :
        base(
            userId,
            AccountEnums.AccountKeys.BANK,
            bankAccountType,
            InvestmentAccountEnums.InvestmentAccountKeys.NA,
            LoanAccountEnums.LoanAccountKeys.NA,
            name, description, displayColor)
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
        decimal overdraft) :
        base(
            userId,
            BankAccountEnums.BankAccountKeys.CHECKING,
            name, description, displayColor)
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
        decimal interestRate) :
        base(
            userId,
            BankAccountEnums.BankAccountKeys.SAVINGS,
            name, description, displayColor)
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
        decimal interestRate) :
        base(
            userId,
            BankAccountEnums.BankAccountKeys.CREDITLINE,
            name, description, displayColor)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;
    }

    //public static explicit operator CreateAccountCommand(CreateCreditLineAccountRequest request)
    //{
    //    var command = new CreateAccountCommand(
    //        request.UserId,
    //        request.AccountType,
    //        request.Name,
    //        request.Description,
    //        request.DisplayColor,
    //        request.BankAccountType,
    //        request.LoanAccountType,
    //        CreditLimit: request.CreditLimit,
    //        InterestRate: request.InterestRate);

    //    return command;
    //}
}

public record CreateCreditCardAccountRequest : CreateAccountRequest
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
        decimal interestRate) :
        base(
            userId,
            AccountEnums.AccountKeys.CREDITCARD,
            BankAccountEnums.BankAccountKeys.NA,
            InvestmentAccountEnums.InvestmentAccountKeys.NA,
            LoanAccountEnums.LoanAccountKeys.NA,
            name, description, displayColor)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;
    }

    //public static explicit operator CreateAccountCommand(CreateCreditCardAccountRequest request)
    //{
    //    var command = new CreateAccountCommand(
    //        request.UserId,
    //        request.AccountType,
    //        request.Name,
    //        request.Description,
    //        request.DisplayColor,
    //        request.BankAccountType,
    //        request.LoanAccountType,
    //        CreditLimit: request.CreditLimit,
    //        InterestRate: request.InterestRate);

    //    return command;
    //}
}

public record CreateLoanAccountRequest : CreateAccountRequest
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
        decimal interestRate) :
        base(
            userId,
            AccountEnums.AccountKeys.LOAN,
            BankAccountEnums.BankAccountKeys.NA,
            InvestmentAccountEnums.InvestmentAccountKeys.NA,
            loanAccountType,
            name, description, displayColor)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;

        LoanAccountType = loanAccountType;
    }

    //public static explicit operator CreateAccountCommand(CreateLoanAccountRequest request)
    //{
    //    var command = new CreateAccountCommand(
    //        request.UserId,
    //        request.AccountType,
    //        request.Name,
    //        request.Description,
    //        request.DisplayColor,
    //        request.BankAccountType,
    //        request.LoanAccountType,
    //        CreditLimit: request.CreditLimit,
    //        InterestRate: request.InterestRate);

    //    return command;
    //}
}