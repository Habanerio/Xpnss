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
    public List<ChangeHistory> ChangeHistory { get; set; } = new();

    [BsonElement("monthly_deposit_totals")]
    public List<MonthlyTotal> MonthlyDepositTotals { get; set; } = new();

    [BsonElement("monthly_withdraw_totals")]
    public List<MonthlyTotal> MonthlyWithdrawTotals { get; set; } = new();

    public AccountDocument()
    {
        Id = ObjectId.GenerateNewId();
        DateCreated = DateTime.UtcNow;
    }

    public void AddChangeHistory(string userId, string property, string oldValue, string newValue, DateTime dateChanged, string reason)
    {
        ChangeHistory.Add(new ChangeHistory
        {
            AccountId = Id.ToString(),
            UserId = userId,
            Property = property,
            OldValues = oldValue,
            NewValues = newValue,
            Reason = reason,
            DateChanged = dateChanged.Date,
        });
    }

    public virtual void Deposit(DateTime date, decimal amount)
    {
        var monthlyTotal = MonthlyDepositTotals.Find(x => x.Year == date.Year && x.Month == date.Month);

        if (monthlyTotal == null)
        {
            monthlyTotal = new MonthlyTotal
            {
                Year = date.Year,
                Month = date.Month,
                Total = amount,
                TransactionCount = 1,
            };

            MonthlyDepositTotals.Add(monthlyTotal);
        }
        else
        {
            monthlyTotal.Total += amount;
            monthlyTotal.TransactionCount++;
        }

        UpdateBalanceFromHistory();
    }

    public virtual void Withdraw(DateTime date, decimal amount)
    {
        var monthlyTotal = MonthlyWithdrawTotals.Find(x => x.Year == date.Year && x.Month == date.Month);

        if (monthlyTotal == null)
        {
            monthlyTotal = new MonthlyTotal
            {
                Year = date.Year,
                Month = date.Month,
                Total = amount,
                TransactionCount = 1,
            };

            MonthlyWithdrawTotals.Add(monthlyTotal);
        }
        else
        {
            monthlyTotal.Total += amount;
            monthlyTotal.TransactionCount++;
        }

        UpdateBalanceFromHistory();
    }

    /// <summary>
    /// Removes an amount from one or the other Monthly Totals.
    /// This is used when a transaction is deleted.
    /// </summary>
    /// <param name="isDeposit"></param>
    /// <param name="date"></param>
    /// <param name="amount"></param>
    public void RemoveFromMonthlyTotals(bool isDeposit, DateTime date, decimal amount)
    {
        var monthlyTotals = isDeposit ? MonthlyDepositTotals : MonthlyWithdrawTotals;

        var monthlyTotal = monthlyTotals.Find(x => x.Year == date.Year && x.Month == date.Month);

        if (monthlyTotal != null)
        {
            monthlyTotal.Total += amount;
            monthlyTotal.TransactionCount--;

            UpdateBalanceFromHistory();
        }
    }

    internal void UpdateBalanceFromHistory()
    {
        var deposits = MonthlyDepositTotals.Sum(x => x.Total);
        var withdraws = MonthlyWithdrawTotals.Sum(x => x.Total);

        //var balanceHistories = ChangeHistory.FindAll(x => x.Property == "Balance");

        Balance = deposits - withdraws;
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

public class CheckingAccount : CashAccount
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

public class SavingsAccount : CashAccount, IHasInterestRate
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
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime DateChanged { get; set; }
}

public sealed class MonthlyTotal
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Total { get; set; }
    public int TransactionCount { get; set; }
}