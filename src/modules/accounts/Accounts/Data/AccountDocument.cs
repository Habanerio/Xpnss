using Habanerio.Core.DBs.MongoDB.EFCore;
using Habanerio.Xpnss.Modules.Accounts.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Xpnss.Modules.Accounts.Data;

//[Table("MoneyAccount")]
//[Collection("money_accounts")]
public class AccountDocument : MongoDocument
{

    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("account_type")]
    [BsonRepresentation(BsonType.String)]
    public AccountType AccountType { get; set; }

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
    public DateTimeOffset DateCreated { get; set; }

    [BsonElement("date_updated")]
    public DateTimeOffset? DateUpdated { get; set; }

    [BsonElement("date_deleted")]
    public DateTimeOffset? DateDeleted { get; set; }

    [BsonElement("extended_props")]
    public List<KeyValuePair<string, object?>> ExtendedProps { get; set; } = [];

    //[BsonElement("change_history")]
    public List<ChangeHistory> ChangeHistory { get; set; }

    [BsonElement("monthly_totals")]
    public List<MonthlyTotal> MonthlyTotals { get; set; }

    public AccountDocument()
    {
        ChangeHistory = [];
        MonthlyTotals = [];
    }

    public static AccountDocument New(
        string userId,
        string name,
        AccountType accountType,
        string description,
        decimal balance,
        string displayColor)
    {
        return new AccountDocument
        {
            Id = ObjectId.GenerateNewId(),
            UserId = userId,
            AccountType = accountType,
            Name = name,
            Balance = balance,
            DisplayColor = displayColor,
            Description = description,
            DateCreated = DateTimeOffset.UtcNow,
            ChangeHistory = [],
        };
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

/// <summary>
/// Used to keep track of changes, such as Balance, Credit Limit, Interest Rate Adjustments.
/// Not for updates such as Name, Description, or Display Color changes.
/// </summary>
public sealed class ChangeHistory
{
    //[BsonElement("account_id")]
    public string AccountId { get; set; }

    //[BsonElement("user_id")]
    public string UserId { get; set; } = "";

    //[BsonElement("old_value")]
    public string? OldValues { get; set; } = null;

    //[BsonElement("new_value")]
    public string NewValues { get; set; } = "";

    //[BsonElement("property")]
    public string Property { get; set; } = "";

    //[BsonElement("reason")]
    public string Reason { get; set; } = "";

    //[BsonElement("date_changed")]
    public DateTime DateChanged { get; set; }
}

public sealed class MonthlyTotal
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Total { get; set; }
    public int TransactionCount { get; set; }
}