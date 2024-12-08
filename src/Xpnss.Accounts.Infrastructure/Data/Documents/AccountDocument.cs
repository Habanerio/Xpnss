using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Domain.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;

[BsonCollection("money_accounts")]
public partial class AccountDocument :
    MongoDocument//, IMongoDocument
{
    [BsonElement("user_id")]
    public ObjectId UserId { get; set; }


    /// <summary>
    /// Account AccountType
    /// </summary>
    [BsonElement("ofx_account_type")]
    [BsonRepresentation(BsonType.String)]
    public AccountEnums.AccountKeys AccountType { get; set; }

    /// <summary>
    /// Bank Account AccountType
    /// </summary>
    [BsonElement("ofx_bank_account_type")]
    [BsonRepresentation(BsonType.String)]
    public BankAccountEnums.BankAccountKeys BankAccountType { get; set; } =
        BankAccountEnums.BankAccountKeys.NA;

    /// <summary>
    /// Investment Account AccountType
    /// </summary>
    [BsonElement("ofx_investment_account_type")]
    [BsonRepresentation(BsonType.String)]
    public InvestmentAccountEnums.InvestmentAccountKeys InvestmentAccountType { get; set; } =
        InvestmentAccountEnums.InvestmentAccountKeys.NA;

    /// <summary>
    /// Loan Account AccountType
    /// </summary>
    [BsonElement("ofx_loan_account_type")]
    [BsonRepresentation(BsonType.String)]
    public LoanAccountEnums.LoanAccountKeys LoanAccountType { get; set; } =
        LoanAccountEnums.LoanAccountKeys.NA;

    [BsonElement("institution_name")]
    public string InstitutionName { get; set; } = string.Empty;

    /// <summary>
    /// Bank Account, Credit Card Account, Investment Account, Presentment Account, Loan Account(?) ExtAcctId
    /// </summary>
    [BsonElement("ofx_account_id")]
    public string ExtAcctId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the specific Account type
    /// </summary>
    /// <example>Capital One (Credit Card)</example>
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("balance")]
    public decimal Balance { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("display_color")]
    public string DisplayColor { get; set; } = string.Empty;


    [BsonElement("credit_limit")]
    public decimal CreditLimit { get; set; }

    [BsonElement("interest_rate")]
    public decimal InterestRate { get; set; }

    [BsonElement("overdraft_limit")]
    public decimal OverdraftLimit { get; set; }

    [BsonElement("is_closed")]
    public bool IsClosed => ClosedDate.HasValue;

    [BsonElement("is_deleted")]
    public bool IsDeleted => DateDeleted.HasValue;

    [BsonElement("is_credit")]
    public bool IsCredit { get; set; }

    [BsonElement("is_default")]
    public bool IsDefault { get; set; }

    [BsonElement("is_over_limit")]
    public bool IsOverLimit { get; set; }


    [BsonElement("sort_order")]
    public int SortOrder { get; set; }


    [BsonElement("closed_date")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime? ClosedDate { get; set; }


    [BsonElement("date_created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateCreated { get; set; }

    [BsonElement("date_updated")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateUpdated { get; set; }

    [BsonElement("date_deleted")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateDeleted { get; set; }

    [BsonConstructor]
    public AccountDocument()
    { }

    //[BsonConstructor]
    public AccountDocument(
        ObjectId id,
        ObjectId userId,
        AccountEnums.AccountKeys accountType,
        BankAccountEnums.BankAccountKeys bankAccountType,
        InvestmentAccountEnums.InvestmentAccountKeys investmentAccountType,
        LoanAccountEnums.LoanAccountKeys loanAccountType,
        string extAcctId,
        string accountName,
        decimal balance,
        string description,
        string displayColor,
        bool isCredit,
        bool isDefault,
        DateTime? closedDate,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        Id = id;
        UserId = userId;
        AccountType = accountType;
        BankAccountType = bankAccountType;
        InvestmentAccountType = investmentAccountType;
        LoanAccountType = loanAccountType;
        ExtAcctId = extAcctId;
        ClosedDate = closedDate;
        IsCredit = isCredit;
        IsDefault = isDefault;
        Name = accountName;
        Balance = balance;
        Description = description;
        DisplayColor = displayColor;
        IsCredit = isCredit;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;
    }
}