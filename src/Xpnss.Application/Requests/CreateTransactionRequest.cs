using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.Requests;

public record CreateTransactionRequest
{
    private DateTime _transactionDate;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string AccountId { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ExtTransactionId { get; set; } = "";

    public bool? IsCredit { get; set; } = false;

    public PayerPayeeRequest PayerPayee { get; set; } = new();

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
    public CreateTransactionRequest() { }

    protected CreateTransactionRequest(bool? isCredit, TransactionEnums.TransactionKeys transactionType)
    {
        IsCredit = isCredit;
        TransactionType = transactionType;
    }
}

#region - Credit Transactions -

public abstract record CreateCreditTransactionRequest :
    CreateTransactionRequest
{
    protected CreateCreditTransactionRequest(TransactionEnums.TransactionKeys transactionType) :
        base(true, transactionType)
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
        string accountId,
        decimal amount,
        string description,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(TransactionEnums.TransactionKeys.DEPOSIT)
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
public sealed record CreateRefundTransactionRequest(string CategoryId) :
    CreateCreditTransactionRequest(TransactionEnums.TransactionKeys.REFUND_REIMBURSEMENT)
{ }

/// <summary>
/// For refunds where there is a purchase transaction to refund against
/// </summary>
public sealed record CreateRefundPurchaseTransactionRequest(
    string PurchaseTransactionId,
    List<PurchaseTransactionItemRequest> Items) :
    CreateCreditTransactionRequest(TransactionEnums.TransactionKeys.REFUND_REIMBURSEMENT)
{ }

#endregion // Credit Transactions

#region - Debit Transactions -

public abstract record CreateDebitTransactionRequest :
    CreateTransactionRequest
{
    protected CreateDebitTransactionRequest(TransactionEnums.TransactionKeys transactionType) :
        base(false, transactionType)
    { }
}

public sealed record CreatePurchaseTransactionRequest :
    CreateDebitTransactionRequest
{
    public List<PurchaseTransactionItemRequest> Items { get; set; } = [];

    public override decimal TotalAmount => Items.Sum(i => i.Amount);

    [JsonConstructor]
    public CreatePurchaseTransactionRequest() :
        base(TransactionEnums.TransactionKeys.PURCHASE)
    { }

    public CreatePurchaseTransactionRequest(
        string accountId,
        PayerPayeeRequest payee,
        string description,
        DateTime transactionDate,
        List<PurchaseTransactionItemRequest> items,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(TransactionEnums.TransactionKeys.PURCHASE)
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
public sealed record CreateWithdrawalTransactionRequest :
    CreateDebitTransactionRequest
{
    [JsonConstructor]
    public CreateWithdrawalTransactionRequest() :
        base(TransactionEnums.TransactionKeys.WITHDRAWAL)
    { }

    public CreateWithdrawalTransactionRequest(
        string accountId,
        decimal amount,
        PayerPayeeRequest payee,
        string description,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(TransactionEnums.TransactionKeys.WITHDRAWAL)
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
public sealed record PurchaseTransactionItemRequest
{
    public decimal Amount { get; set; }

    public string CategoryId { get; set; } = "";

    public string SubCategoryId { get; set; } = "";

    public string Description { get; set; } = "";
}