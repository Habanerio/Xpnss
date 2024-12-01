using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Infrastructure.Interfaces.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;

[BsonCollection("money_accounts")]
public class AccountDocument : MongoDocument//, IMongoDocument
{
    [BsonElement("user_id")]
    public ObjectId UserId { get; set; }

    [BsonElement("account_type")]
    [BsonRepresentation(BsonType.String)]
    public AccountTypes.Keys AccountType { get; set; }

    /// <summary>
    /// Name of the specific Account type
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

    [BsonElement("is_closed")]
    public bool IsClosed { get; set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }

    [BsonElement("date_closed")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateClosed { get; set; }

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
    {
        //AdjustmentHistories = [];
    }

    //[BsonConstructor]
    public AccountDocument(ObjectId id)
    {
        Id = id;
        DateCreated = DateTime.UtcNow;
        //AdjustmentHistories = [];
    }
}


public class CashAccountDocument : AccountDocument
{
    public CashAccountDocument(ObjectId id) : base(id)
    {
        AccountType = AccountTypes.Keys.CASH;
    }
}

public sealed class CheckingAccountDocument : CashAccountDocument, IDocumentHasOverdraftAmount
{
    [BsonElement("overdraft_limit")]
    public decimal OverdraftAmount { get; set; }

    public CheckingAccountDocument(ObjectId id, decimal overDraftAmount) : base(id)
    {
        AccountType = AccountTypes.Keys.CHECKING;
        OverdraftAmount = overDraftAmount;
    }
}

public sealed class SavingsAccountDocument : CashAccountDocument, IDocumentHasInterestRate
{
    [BsonElement("interest_rate")]
    public decimal InterestRate { get; set; }

    public SavingsAccountDocument(ObjectId id, decimal interestRate) : base(id)
    {
        AccountType = AccountTypes.Keys.SAVINGS;
        InterestRate = interestRate;
    }
}

public abstract class CreditAccountDocument : AccountDocument, IDocumentHasCreditLimit, IDocumentHasInterestRate
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
    : CreditAccountDocument(id, AccountTypes.Keys.CREDIT_CARD, creditLimit, interestRate);

public sealed class LineOfCreditAccountDocument(ObjectId id, decimal creditLimit, decimal interestRate)
    : CreditAccountDocument(id, AccountTypes.Keys.LINE_OF_CREDIT, creditLimit, interestRate);
