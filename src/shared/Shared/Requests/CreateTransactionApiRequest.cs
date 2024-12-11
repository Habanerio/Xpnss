using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Shared.Requests;

public record CreateTransactionApiRequest : UserRequiredApiRequest
{
    private DateTime _transactionDate;

    [Required]
    public string AccountId { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ExtTransactionId { get; init; } = string.Empty;

    public bool IsCredit { get; init; }

    public PayerPayeeApiRequest PayerPayee { get; init; } = new();

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
    public CreateTransactionApiRequest() { }

    /// <summary>
    /// JsonConstructor for the derived types
    /// </summary>
    internal CreateTransactionApiRequest(bool isCredit, TransactionEnums.TransactionKeys transactionType)
    {
        IsCredit = isCredit;
        TransactionType = transactionType;
    }

    protected CreateTransactionApiRequest(
        string userId,
        TransactionEnums.TransactionKeys transactionType,
        string accountId,
        decimal amount,
        string description,
        string extTransactionId,
        bool isCredit,
        string payerPayeeName,
        List<string>? tags,
        DateTime transactionDate)
    {
        UserId = userId;
        AccountId = accountId;
        TotalAmount = amount;
        Description = description;
        IsCredit = isCredit;
        ExtTransactionId = extTransactionId;
        PayerPayee = new PayerPayeeApiRequest
        {
            Name = payerPayeeName
        };
        Tags = tags ?? [];
        TransactionDate = transactionDate;
        TransactionType = transactionType;
    }
}

#region - Credit Transactions -

public record CreateCreditTransactionApiRequest :
    CreateTransactionApiRequest
{
    [JsonConstructor]
    protected CreateCreditTransactionApiRequest(TransactionEnums.TransactionKeys transactionType) :
        base(true, transactionType)
    { }

    protected CreateCreditTransactionApiRequest(
        string userId,
        TransactionEnums.TransactionKeys transactionType,
        string accountId,
        decimal amount,
        string description,
        string payerPayeeName,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(
            userId,
            transactionType,
            accountId,
            amount,
            description,
            extTransactionId,
            true,
            payerPayeeName,
            tags,
            transactionDate)
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
        string payerPayeeName,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(
            userId,
            TransactionEnums.TransactionKeys.DEPOSIT,
            accountId,
            amount,
            description,
            payerPayeeName,
            transactionDate,
            tags,
            extTransactionId)
    { }
}

/// <summary>
/// For refunds where there is no purchase transaction to refund against
/// </summary>
//public sealed record CreateRefundTransactionApiRequest :
//    CreateCreditTransactionApiRequest
//{
//    [JsonConstructor]
//    public CreateRefundTransactionApiRequest() :
//        base(TransactionEnums.TransactionKeys.DEPOSIT)
//    { }


//    public CreateRefundTransactionApiRequest(
//        string userId,
//        string accountId,
//        decimal amount,
//        string description,
//        string payerPayeeName,
//        DateTime transactionDate,
//        List<string>? tags = null,
//        string extTransactionId = "") :
//        base(
//            userId,
//            TransactionEnums.TransactionKeys.REFUND_REIMBURSEMENT,
//            accountId,
//            amount,
//            description,
//            payerPayeeName,
//            transactionDate,
//            tags,
//            extTransactionId)
//    { }
//}

///// <summary>
///// For refunds where there is a purchase transaction to refund against
///// </summary>
//public sealed record CreateRefundPurchaseTransactionApiRequest(
//    string PurchaseTransactionId,
//    List<TransactionApiRequestItem> Items) :
//    CreateCreditTransactionApiRequest(TransactionEnums.TransactionKeys.REFUND_REIMBURSEMENT)
//{ }

#endregion // Credit Transactions

#region - Debit Transactions -

public abstract record CreateDebitTransactionApiRequest :
    CreateTransactionApiRequest
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
        string payerPayeeName,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(
            userId,
            transactionType,
            accountId,
            amount,
            description,
            extTransactionId,
            false,
            payerPayeeName,
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
        PayerPayeeApiRequest payee,
        string description,
        DateTime transactionDate,
        List<TransactionApiRequestItem> items,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(TransactionEnums.TransactionKeys.PURCHASE)
    {
        UserId = userId;
        AccountId = accountId;
        PayerPayee = payee;
        Description = description;
        ExtTransactionId = extTransactionId;
        TransactionDate = transactionDate;
        Items = items;
        Tags = tags ?? [];
        TotalAmount = items.Sum(i => i.Amount);
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
        string cashAccountId,
        string description,
        DateTime transactionDate,
        List<string>? tags = null,
        string extTransactionId = "") :
        base(
            userId,
            TransactionEnums.TransactionKeys.WITHDRAWAL,
            accountId,
            amount,
            description,
            cashAccountId,
            transactionDate,
            tags,
            extTransactionId)
    {
        if (string.IsNullOrWhiteSpace(cashAccountId))
            throw new ArgumentException("Cash Account Id is required for Withdrawal Transactions. " +
                                        "The money has to go somewhere");
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