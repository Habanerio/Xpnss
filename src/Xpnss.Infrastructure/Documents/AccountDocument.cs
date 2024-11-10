using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Domain.Accounts;
using Habanerio.Xpnss.Infrastructure.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Infrastructure.Documents;

[BsonCollection("money_accounts")]
public class AccountDocument : MongoDocument//, IMongoDocument
{
    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("account_type")]
    [BsonRepresentation(BsonType.String)]
    public AccountTypes.Keys AccountType { get; set; }

    /// <summary>
    /// Name of the specific account type
    /// </summary>
    /// <example>Capital One (Credit Card)</example>
    [BsonElement("account_name")]
    public string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = "";

    [BsonElement("display_color")]
    public string DisplayColor { get; set; } = "";


    [BsonElement("balance")]
    public decimal Balance { get; set; }

    [BsonElement("is_default")]
    public bool IsDefault { get; set; }

    [BsonElement("is_credit")]
    public bool IsCredit { get; set; }


    [BsonElement("date_created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateCreated { get; set; }

    [BsonElement("date_updated")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateUpdated { get; set; }

    [BsonElement("date_closed")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateClosed { get; set; }

    [BsonElement("date_deleted")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateDeleted { get; set; }

    [BsonConstructor]
    public AccountDocument()
    {

    }

    //[BsonConstructor]
    public AccountDocument(ObjectId id)
    {
        Id = id;
        DateCreated = DateTime.UtcNow;
    }

    //protected AccountDocument(string userId, AccountTypes.Keys accountType, string name, string description, string displayColor, decimal balance, bool isDefault, bool isCredit)
    //{
    //    Id = ObjectId.GenerateNewId();
    //    UserId = userId;
    //    AccountType = accountType;
    //    Name = name;
    //    Description = description;
    //    DisplayColor = displayColor;
    //    Balance = balance;
    //    IsDefault = isDefault;
    //    IsCredit = isCredit;
    //    DateCreated = DateTime.UtcNow;
    //}

    //protected AccountDocument(string id, string userId, AccountTypes.Keys accountType, string name, string description, string displayColor, decimal balance, bool isDefault, bool isCredit, DateTime dateCreated, DateTime dateUpdated, DateTime dateClosed, DateTime dateDeleted)
    //{
    //    Id = ObjectId.GenerateNewId();
    //    UserId = userId;
    //    AccountType = accountType;
    //    Name = name;
    //    Description = description;
    //    DisplayColor = displayColor;
    //    Balance = balance;
    //    IsDefault = isDefault;
    //    IsCredit = isCredit;
    //    DateCreated = dateCreated;
    //    DateUpdated = dateUpdated;
    //    DateClosed = dateClosed;
    //    DateDeleted = dateDeleted;
    //}
}


public class CashAccountDocument : AccountDocument
{
    public CashAccountDocument(ObjectId id) : base(id)
    {
        AccountType = AccountTypes.Keys.Cash;
    }
}

public sealed class CheckingAccountDocument : CashAccountDocument
{
    [BsonElement("overdraft_limit")]
    public decimal OverDraftAmount { get; set; }

    public CheckingAccountDocument(ObjectId id, decimal overDraftAmount) : base(id)
    {
        AccountType = AccountTypes.Keys.Checking;
        OverDraftAmount = overDraftAmount;
    }
}

public sealed class SavingsAccountDocument : CashAccountDocument, IHasInterestRate
{
    [BsonElement("interest_rate")]
    public decimal InterestRate { get; set; }

    public SavingsAccountDocument(ObjectId id, decimal interestRate) : base(id)
    {
        AccountType = AccountTypes.Keys.Savings;
        InterestRate = interestRate;
    }
}

public abstract class CreditAccountDocument : AccountDocument, IHasCreditLimit, IHasInterestRate
{
    protected CreditAccountDocument(ObjectId id, AccountTypes.Keys accountType, decimal creditLimit, decimal interestRate) : base(id)
    {
        CreditLimit = creditLimit;
        InterestRate = interestRate;
        IsCredit = true;
    }

    [BsonElement("credit_limit")]
    public decimal CreditLimit { get; set; }

    [BsonElement("interest_rate")]
    public decimal InterestRate { get; set; }
}

public sealed class CreditCardAccountDocument(ObjectId id, decimal creditLimit, decimal interestRate)
    : CreditAccountDocument(id, AccountTypes.Keys.CreditCard, creditLimit, interestRate);

public sealed class LineOfCreditAccountDocument(ObjectId id, decimal creditLimit, decimal interestRate)
    : CreditAccountDocument(id, AccountTypes.Keys.LineOfCredit, creditLimit, interestRate);
