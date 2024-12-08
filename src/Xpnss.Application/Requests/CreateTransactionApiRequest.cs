using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.Requests;

public record CreateTransactionApiRequest : UserRequiredApiRequest
{
    private DateTime _transactionDate;

    [Required]
    public string AccountId { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ExtTransactionId { get; set; } = string.Empty;

    public bool? IsCredit { get; set; } = false;

    public PayerPayeeApiRequest PayerPayee { get; set; } = new();

    public List<string> Tags { get; set; } = [];

    [Required]
    public virtual decimal TotalAmount { get; set; }

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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionEnums.TransactionKeys TransactionType { get; set; }


    [JsonConstructor]
    public CreateTransactionApiRequest() { }

    /// <summary>
    /// JsonConstructor for the derived types
    /// </summary>
    internal CreateTransactionApiRequest(bool? isCredit, TransactionEnums.TransactionKeys transactionType)
    {
        IsCredit = isCredit;
        TransactionType = transactionType;
    }

    internal CreateTransactionApiRequest(string userId, bool? isCredit, TransactionEnums.TransactionKeys transactionType)
    {
        UserId = userId;
        IsCredit = isCredit;
        TransactionType = transactionType;
    }
}

#region - Credit Transactions -

public abstract record CreateCreditTransactionApiRequest :
    CreateTransactionApiRequest
{
    [JsonConstructor]
    protected CreateCreditTransactionApiRequest(TransactionEnums.TransactionKeys transactionType) :
        base(true, transactionType)
    { }

    protected CreateCreditTransactionApiRequest(string userId, TransactionEnums.TransactionKeys transactionType) :
        base(userId, true, transactionType)
    { }
}

/// <summary>
/// Represents when the user deposits money into their account from an external source
/// (eg: Income, Gift, etc)
/// </summary>
public sealed record CreateDepositTransactionApiRequest :
    CreateCreditTransactionApiRequest
{
    [JsonConstructor]
    public CreateDepositTransactionApiRequest() :
        base(TransactionEnums.TransactionKeys.DEPOSIT)
    { }

    public CreateDepositTransactionApiRequest(
        string userId,
        string accountId,
        decimal amount,
        string description,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(userId, TransactionEnums.TransactionKeys.DEPOSIT)
    {
        AccountId = accountId;
        TotalAmount = amount;
        Description = description;
        ExtTransactionId = extTransactionId;
        Tags = tags ?? [];
        TransactionDate = transactionDate;
    }
}

/// <summary>
/// For refunds where there is no purchase transaction to refund against
/// </summary>
public sealed record CreateRefundTransactionApiRequest(string CategoryId) :
    CreateCreditTransactionApiRequest(TransactionEnums.TransactionKeys.REFUND_REIMBURSEMENT)
{ }

/// <summary>
/// For refunds where there is a purchase transaction to refund against
/// </summary>
public sealed record CreateRefundPurchaseTransactionApiRequest(
    string PurchaseTransactionId,
    List<TransactionApiRequestItem> Items) :
    CreateCreditTransactionApiRequest(TransactionEnums.TransactionKeys.REFUND_REIMBURSEMENT)
{ }

#endregion // Credit Transactions

#region - Debit Transactions -

public abstract record CreateDebitTransactionApiRequest :
    CreateTransactionApiRequest
{
    [JsonConstructor]
    protected CreateDebitTransactionApiRequest(TransactionEnums.TransactionKeys transactionType) :
        base(false, transactionType)
    { }

    protected CreateDebitTransactionApiRequest(string userId, TransactionEnums.TransactionKeys transactionType) :
        base(userId, false, transactionType)
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
        PayerPayeeApiRequest payee,
        string description,
        DateTime transactionDate,
        List<TransactionApiRequestItem> items,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(userId, TransactionEnums.TransactionKeys.PURCHASE)
    {
        AccountId = accountId;
        PayerPayee = payee;
        Description = description;
        TransactionDate = transactionDate;
        Items = items;
        Tags = tags ?? [];
        ExtTransactionId = extTransactionId;
    }
}

/// <summary>
/// Usually for when then money is taken out as Cash
/// </summary>
public sealed record CreateWithdrawalTransactionApiRequest :
    CreateDebitTransactionApiRequest
{
    [JsonConstructor]
    public CreateWithdrawalTransactionApiRequest() :
        base(TransactionEnums.TransactionKeys.WITHDRAWAL)
    { }

    public CreateWithdrawalTransactionApiRequest(
        string userId,
        string accountId,
        decimal amount,
        PayerPayeeApiRequest payee,
        string description,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(userId, TransactionEnums.TransactionKeys.WITHDRAWAL)
    {
        AccountId = accountId;
        TotalAmount = amount;
        Description = description;
        ExtTransactionId = extTransactionId;
        PayerPayee = payee;
        Tags = tags ?? [];
        TransactionDate = transactionDate;
        TransactionType = TransactionEnums.TransactionKeys.WITHDRAWAL;
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