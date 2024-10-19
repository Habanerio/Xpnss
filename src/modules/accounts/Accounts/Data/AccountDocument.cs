using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Attributes;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Modules.Accounts.Data;

[BsonCollection("money_accounts")]
public class AccountDocument : MongoDocument//, IMongoDocument
{

    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("account_type")]
    [BsonRepresentation(BsonType.String)]
    public AccountTypes AccountType { get; set; }

    /// <summary>
    /// Name of the specific account type
    /// </summary>
    /// <example>Capital One (Credit Card)</example>
    [BsonElement("account_name")]
    public string Name { get; set; }

    [BsonElement("balance")]
    public decimal Balance { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = "";

    [BsonElement("display_color")]
    public string DisplayColor { get; set; } = "";

    [BsonElement("is_credit")]
    public bool IsCredit { get; set; }

    [BsonElement("is_default")]
    public bool IsDefault { get; set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }

    [BsonElement("date_created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateCreated { get; set; }

    [BsonElement("date_updated")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateUpdated { get; set; }

    [BsonElement("date_deleted")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DateDeleted { get; set; }

    [BsonElement("change_history")]
    public List<ChangeHistory> ChangeHistory { get; set; }

    [BsonElement("monthly_totals")]
    public List<MonthlyTotal> MonthlyTotals { get; set; }


    public AccountDocument()
    {
        Id = ObjectId.GenerateNewId();
        ChangeHistory = [];
        MonthlyTotals = [];
        DateCreated = DateTime.UtcNow;
    }

    public void AddChangeHistory(string userId, string property, string oldValue, string newValue, string reason)
    {
        ChangeHistory.Add(new ChangeHistory
        {
            AccountId = Id.ToString(),
            UserId = userId,
            Property = property,
            OldValues = oldValue,
            NewValues = newValue,
            Reason = reason,
            DateChanged = DateTime.UtcNow,
        });
    }
}

public class CashAccount : AccountDocument
{
    public static CashAccount New(
        string userId,
        string name,
        string description,
        decimal balance,
        string displayColor)
    {
        return new CashAccount
        {
            Id = ObjectId.GenerateNewId(),
            UserId = userId,
            AccountType = AccountTypes.Cash,
            Name = name,
            Balance = balance,
            DisplayColor = displayColor,
            Description = description,
            ChangeHistory = [],
        };
    }
}

public class CheckingAccount : AccountDocument
{
    [BsonElement("overdraft_limit")]
    public decimal OverDraftAmount { get; set; }

    public static CheckingAccount New(
    string userId,
    string name,
    string description,
    decimal balance,
    decimal overDraftAmount,
    string displayColor)
    {
        return new CheckingAccount
        {
            Id = ObjectId.GenerateNewId(),
            UserId = userId,
            AccountType = AccountTypes.Checking,
            Name = name,
            Balance = balance,
            DisplayColor = displayColor,
            Description = description,
            OverDraftAmount = overDraftAmount,
            ChangeHistory = [],
        };
    }
}

public class SavingsAccount : AccountDocument, IHasInterestRate
{
    [BsonElement("interest_rate")]
    public decimal InterestRate { get; set; }

    public static SavingsAccount New(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal interestRate,
        string displayColor)
    {
        return new SavingsAccount
        {
            Id = ObjectId.GenerateNewId(),
            UserId = userId,
            AccountType = AccountTypes.Savings,
            Name = name,
            Balance = balance,
            DisplayColor = displayColor,
            Description = description,
            InterestRate = interestRate,
            ChangeHistory = [],
        };
    }
}

public abstract class CreditAccount : AccountDocument, IHasCreditLimit, IHasInterestRate
{
    public CreditAccount()
    {
        IsCredit = true;
    }

    [BsonElement("credit_limit")]
    public decimal CreditLimit { get; set; }

    [BsonElement("interest_rate")]
    public decimal InterestRate { get; set; }
}

public class CreditCardAccount : CreditAccount
{
    public static CreditCardAccount New(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor)
    {
        return new CreditCardAccount
        {
            Id = ObjectId.GenerateNewId(),
            UserId = userId,
            AccountType = AccountTypes.CreditCard,
            Name = name,
            Balance = balance,
            DisplayColor = displayColor,
            Description = description,
            CreditLimit = creditLimit,
            InterestRate = interestRate,
            ChangeHistory = [],
        };
    }
}

public class LineOfCreditAccount : CreditAccount
{
    public static LineOfCreditAccount New(
        string userId,
        string name,
        string description,
        decimal balance,
        decimal creditLimit,
        decimal interestRate,
        string displayColor)
    {
        return new LineOfCreditAccount
        {
            Id = ObjectId.GenerateNewId(),
            UserId = userId,
            AccountType = AccountTypes.LineOfCredit,
            Name = name,
            Balance = balance,
            DisplayColor = displayColor,
            Description = description,
            CreditLimit = creditLimit,
            InterestRate = interestRate,
            ChangeHistory = [],
        };
    }
}

/// <summary>
/// Used to keep track of changes, such as Balance, Credit Limit, Interest Rate Adjustments.
/// Not for updates such as Name, Description, or Display Color changes.
/// </summary>
public sealed class ChangeHistory
{
    [BsonElement("account_id")]
    public string AccountId { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; } = "";

    [BsonElement("old_value")]
    public string? OldValues { get; set; } = null;

    [BsonElement("new_value")]
    public string NewValues { get; set; } = "";

    [BsonElement("property")]
    public string Property { get; set; } = "";

    [BsonElement("reason")]
    public string Reason { get; set; } = "";

    [BsonElement("date_changed")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DateChanged { get; set; }
}

public sealed class MonthlyTotal
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Total { get; set; }
    public int TransactionCount { get; set; }
}