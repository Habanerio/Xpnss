using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Shared.DTOs;

public sealed record AccountDto
{
    #region - Common Properties -

    public string Id { get; set; }

    public string UserId { get; set; }

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


    public string AccountTypeString => AccountType.ToString();

    public string BankAccountTypeString => BankAccountType.ToString();

    public string InvestmentAccountTypeString => InvestmentAccountType.ToString();

    public string LoanAccountTypeString => LoanAccountType.ToString();


    public string Name { get; set; }

    public decimal Balance { get; set; }

    public DateTime? ClosedDate { get; set; }

    public string Description { get; set; } = string.Empty;

    public string DisplayColor { get; set; } = string.Empty;

    public bool IsCredit { get; set; }

    public bool IsDefault { get; set; }

    public int SortOrder { get; set; }

    #endregion

    #region - Bank Account Properties -

    public string ExtAcctId { get; set; } = string.Empty;

    public string InstitutionName { get; set; } = string.Empty;

    #endregion


    public decimal CreditLimit { get; set; }

    public decimal InterestRate { get; set; }

    public decimal OverdraftLimit { get; set; }


    public bool IsClosed => ClosedDate.HasValue;

    public bool IsDeleted => DateDeleted.HasValue;

    public bool IsOverLimit { get; set; }



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
