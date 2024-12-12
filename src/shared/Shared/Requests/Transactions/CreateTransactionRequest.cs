using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Shared.Requests.Transactions;

public record CreateTransactionRequest : UserRequiredRequest
{
    private DateTime _transactionDate;

    [Required]
    public string AccountId { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ExtTransactionId { get; init; } = string.Empty;

    public bool IsCredit { get; init; }

    public PayerPayeeRequest PayerPayee { get; init; } = new();

    public string RefTransactionId { get; set; } = string.Empty;

    public List<string> Tags { get; init; } = [];

    [Required]
    public virtual decimal TotalAmount { get; init; }

    public DateTime TransactionDate
    {
        get
        {
            return _transactionDate.Date;
        }
        set
        {
            _transactionDate = value.Date;
        }
    }

    [Required]
    [JsonPropertyName("TransactionType")]
    [JsonConverter(typeof(JsonNumberEnumConverter<TransactionEnums.TransactionKeys>))]
    public TransactionEnums.TransactionKeys TransactionType { get; set; }


    [JsonConstructor]
    public CreateTransactionRequest() { }

    /// <summary>
    /// JsonConstructor for the derived types
    /// </summary>
    internal CreateTransactionRequest(bool isCredit, TransactionEnums.TransactionKeys transactionType)
    {
        IsCredit = isCredit;
        TransactionType = transactionType;
    }

    protected CreateTransactionRequest(
        string userId,
        TransactionEnums.TransactionKeys transactionType,
        string accountId,
        decimal amount,
        string description,
        string extTransactionId,
        bool isCredit,
        PayerPayeeRequest payerPayee,
        string refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate)
    {
        UserId = userId;
        AccountId = accountId;
        TotalAmount = amount;
        Description = description;
        IsCredit = isCredit;
        ExtTransactionId = extTransactionId;
        PayerPayee = payerPayee;
        RefTransactionId = refTransactionId;
        Tags = tags?.ToList() ?? [];
        TransactionDate = transactionDate;
        TransactionType = transactionType;
    }
}

#region - Credit Transactions -

public record CreateCreditTransactionRequest :
    CreateTransactionRequest
{
    [JsonConstructor]
    public CreateCreditTransactionRequest(TransactionEnums.TransactionKeys transactionType) :
        base(true, transactionType)
    { }

    public CreateCreditTransactionRequest(
        string userId,
        TransactionEnums.TransactionKeys transactionType,
        string accountId,
        decimal amount,
        string description,
        PayerPayeeRequest payerPayee,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "",
        string refTransactionId = "") :
        base(
            userId,
            transactionType,
            accountId,
            amount,
            description,
            extTransactionId,
            true,
            payerPayee,
            refTransactionId,
            tags,
            transactionDate)
    { }
}

/// <summary>
/// Represents when the user deposits money into their account from an external source
/// (eg: Income, Gift, etc)
/// </summary>
public sealed record CreateDepositTransactionRequest :
    CreateCreditTransactionRequest
{
    [JsonConstructor]
    public CreateDepositTransactionRequest() :
        base(TransactionEnums.TransactionKeys.DEPOSIT)
    { }

    public CreateDepositTransactionRequest(
        string userId,
        string accountId,
        decimal amount,
        string description,
        PayerPayeeRequest depositFrom,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "",
        string refTransactionId = "") :
        base(
            userId,
            TransactionEnums.TransactionKeys.DEPOSIT,
            accountId,
            amount,
            description,
            depositFrom,
            transactionDate,
            tags,
            extTransactionId,
            refTransactionId)
    { }
}

#endregion // Credit Transactions

#region - Debit Transactions -

public abstract record CreateDebitTransactionApiRequest :
    CreateTransactionRequest
{
    protected CreateDebitTransactionApiRequest(TransactionEnums.TransactionKeys transactionType) :
        base(false, transactionType)
    { }

    protected CreateDebitTransactionApiRequest(
        string userId,
        TransactionEnums.TransactionKeys transactionType,
        string accountId,
        decimal amount,
        string description,
        PayerPayeeRequest payerPayee,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "",
        string refTransactionId = "") :
        base(
            userId,
            transactionType,
            accountId,
            amount,
            description,
            extTransactionId,
            false,
            payerPayee,
            refTransactionId,
            tags,
            transactionDate)
    { }
}

public sealed record CreatePurchaseTransactionApiRequest :
    CreateDebitTransactionApiRequest
{
    public List<TransactionApiRequestItem> Items { get; set; } = [];

    public override decimal TotalAmount => Items.Sum(i => i.Amount);

    [JsonConstructor]
    public CreatePurchaseTransactionApiRequest() :
        base(TransactionEnums.TransactionKeys.PURCHASE)
    { }

    public CreatePurchaseTransactionApiRequest(
        string userId,
        string accountId,
        PayerPayeeRequest payee,
        string description,
        DateTime transactionDate,
        List<TransactionApiRequestItem> items,
        IEnumerable<string>? tags = null,
        string extTransactionId = "",
        string refTransactionId = "") :
        base(TransactionEnums.TransactionKeys.PURCHASE)
    {
        UserId = userId;
        AccountId = accountId;
        PayerPayee = payee;
        Description = description;
        ExtTransactionId = extTransactionId;
        TransactionDate = transactionDate;
        Items = items;
        RefTransactionId = refTransactionId;
        Tags = tags?.ToList() ?? [];
        TotalAmount = items.Sum(i => i.Amount);
    }
}

/// <summary>
/// Usually for when then money is taken out as Cash
/// </summary>
public sealed record CreateWithdrawalTransactionRequest :
    CreateDebitTransactionApiRequest
{
    [JsonConstructor]
    public CreateWithdrawalTransactionRequest() :
        base(TransactionEnums.TransactionKeys.WITHDRAWAL)
    { }

    public CreateWithdrawalTransactionRequest(
        string userId,
        string accountId,
        decimal amount,
        string description,
        PayerPayeeRequest withdrewTo,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "",
        string refTransactionId = "") :
        base(
            userId,
            TransactionEnums.TransactionKeys.WITHDRAWAL,
            accountId,
            amount,
            description,
            withdrewTo,
            transactionDate,
            tags,
            extTransactionId,
            refTransactionId)
    {
        //if (string.IsNullOrWhiteSpace(cashAccountId))
        //    throw new ArgumentException("Cash Account Id is required for Withdrawal Transactions. " +
        //        "The money has to go somewhere");
    }
}

#endregion // Debit Transactions

// 
public sealed record TransactionApiRequestItem
{
    public decimal Amount { get; set; }

    public string CategoryId { get; set; } = string.Empty;

    public string SubCategoryId { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}