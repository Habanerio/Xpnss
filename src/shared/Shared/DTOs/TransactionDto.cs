using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Shared.DTOs;

public record TransactionDto
{
    public string Id { get; set; } = "";

    public string UserId { get; set; } = "";

    public string AccountId { get; set; } = "";

    public string Description { get; set; } = string.Empty;

    public string ExtTransactionId { get; set; } = string.Empty;

    public bool IsCredit { get; protected set; }

    public string PayerPayeeId { get; set; } = string.Empty;

    //public PayerPayeeDto PayerPayee { get; set; }

    public List<string> Tags { get; set; } = [];

    public virtual decimal TotalAmount { get; set; }

    public DateTime TransactionDate { get; set; }

    [JsonPropertyName("TransactionType")]
    [JsonConverter(typeof(JsonNumberEnumConverter<TransactionEnums.TransactionKeys>))]
    public TransactionEnums.TransactionKeys TransactionType { get; protected set; }

    public string TransactionTypeString => TransactionType.ToString();

    [JsonConstructor]
    protected TransactionDto(
        bool isCredit,
        TransactionEnums.TransactionKeys transactionType)
    {
        IsCredit = isCredit;
        TransactionType = transactionType;
    }

    protected TransactionDto(
        string userId,
        string accountId,
        string description,
        string extTransactionId,
        bool isCredit,
        string payerPayeeId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType)
    {
        UserId = userId;
        AccountId = accountId;
        Description = description;
        ExtTransactionId = extTransactionId;
        IsCredit = isCredit;
        PayerPayeeId = payerPayeeId;
        Tags = tags?.ToList() ?? [];
        TransactionDate = transactionDate;
        TransactionType = transactionType;
    }
}

#region - Credit Transactions -

/// <summary>
/// A Credit Transaction ("CR") is a transaction that takes money from an ofxAccount.
/// </summary>
public record CreditTransactionDto : TransactionDto
{
    protected CreditTransactionDto(TransactionEnums.TransactionKeys transactionType) :
        base(true, transactionType)
    { }

    public override decimal TotalAmount { get; set; }

    protected CreditTransactionDto(
        string userId,
        string accountId,
        string description,
        string extTransactionId,
        string payerPayeeId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            true,
            payerPayeeId,
            tags,
            transactionDate,
            transactionType)
    {
        TotalAmount = totalAmount;
    }
}

/// <summary>
/// A transaction that adds money to an ofxAccount,
/// such as cash to a checking ofxAccount.
/// </summary>
public sealed record DepositTransactionDto :
    CreditTransactionDto
{
    [JsonConstructor]
    public DepositTransactionDto() :
        base(TransactionEnums.TransactionKeys.DEPOSIT)
    { }

    public DepositTransactionDto(
        string userId,
        string accountId,
        string description,
        string extTransactionId,
        string payerPayeeId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            payerPayeeId,
            tags,
            totalAmount,
            transactionDate,
            TransactionEnums.TransactionKeys.DEPOSIT)
    { }
}

#endregion

#region - Debit Transactions -

/// <summary>
/// A Debit Transaction ("DR") is a transaction that adds money to an ofxAccount.
/// </summary>
public abstract record DebitTransactionDto : TransactionDto
{
    [JsonConstructor]
    protected DebitTransactionDto(TransactionEnums.TransactionKeys transactionType) :
        base(false, transactionType)
    { }

    public override decimal TotalAmount { get; set; }

    protected DebitTransactionDto(
        string userId,
        string accountId,
        string description,
        string extTransactionId,
        string payerPayeeId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            false,
            payerPayeeId,
            tags,
            transactionDate,
            transactionType)
    {
        TotalAmount = totalAmount;
    }
}

public sealed record PurchaseTransactionDto :
    DebitTransactionDto
{
    public bool IsPaid => PaidDate.HasValue;

    public List<TransactionItemDto> Items { get; set; } = [];

    public DateTime? PaidDate { get; set; }

    public override decimal TotalAmount => Items.Sum(i => i.Amount);

    public decimal TotalOwing => TotalAmount - TotalPaid;

    public decimal TotalPaid { get; set; }

    [JsonConstructor]
    public PurchaseTransactionDto() :
        base(TransactionEnums.TransactionKeys.PURCHASE)
    { }

    public PurchaseTransactionDto(
        string userId,
        string accountId,
        string description,
        string extTransactionId,
        IEnumerable<TransactionItemDto> items,
        string payerPayeeId,
        IEnumerable<string>? tags,
        decimal totalPaid,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            payerPayeeId,
            tags,
            0,
            transactionDate,
            TransactionEnums.TransactionKeys.PURCHASE)
    {
        Items = items?.ToList() ?? [];
        TotalPaid = totalPaid;
    }
}

/// <summary>
/// A transaction that takes money out of an ofxAccount,
/// such as a withdrawal from a checking ofxAccount.
/// </summary>
public sealed record WithdrawalTransactionDto :
    DebitTransactionDto
{
    [JsonConstructor]
    public WithdrawalTransactionDto() :
        base(TransactionEnums.TransactionKeys.WITHDRAWAL)
    { }

    ///<summary>
    /// </summary>
    /// <param name="payerPayeeId">The Id of the ofxAccount
    /// that the money withdrew to (Cash/Wallet)</param>
    public WithdrawalTransactionDto(
        string userId,
        string accountId,
        string description,
        string extTransactionId,
        string payerPayeeId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            payerPayeeId,
            tags,
            totalAmount,
            transactionDate,
            TransactionEnums.TransactionKeys.WITHDRAWAL)
    { }
}

public sealed record PaymentTransactionDto :
    DebitTransactionDto
{
    [JsonConstructor]
    public PaymentTransactionDto() :
        base(TransactionEnums.TransactionKeys.PAYMENT)
    { }

    public PaymentTransactionDto(
        string userId,
        string accountId,
        string description,
        string extTransactionId,
        string payerPayeeId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            payerPayeeId,
            tags,
            totalAmount,
            transactionDate,
            TransactionEnums.TransactionKeys.PAYMENT)
    { }
}

#endregion

public sealed record TransactionItemDto
{
    public string Id { get; set; } = string.Empty;

    public string CategoryId { get; set; } = string.Empty;

    public string SubCategoryId { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}