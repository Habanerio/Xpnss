using System.Text.Json.Serialization;

using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Shared.DTOs;

public record TransactionDto
{
    public string Id { get; set; } = "";

    public string UserId { get; set; } = "";

    public string AccountId { get; set; } = "";

    public string CategoryId { get; set; } = "";

    public string Description { get; set; } = string.Empty;

    public string ExtTransactionNo { get; set; } = string.Empty;

    public bool IsCredit { get; set; }

    public string PayerPayeeId { get; set; } = string.Empty;

    //public PayerPayeeDto PayerPayee { get; set; }

    //public string RefTransactionId { get; set; } = string.Empty;

    public string SubCategoryId { get; set; } = "";

    public List<string> Tags { get; set; } = [];

    public virtual decimal TotalAmount { get; set; }

    public DateTime TransactionDate { get; set; }

    [JsonPropertyName("TransactionType")]
    [JsonConverter(typeof(JsonNumberEnumConverter<TransactionEnums.TransactionKeys>))]
    public TransactionEnums.TransactionKeys TransactionType { get; set; }

    public string TransactionTypeString => TransactionType.ToString();

    [JsonConstructor]
    public TransactionDto() { }

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
        string categoryId,
        string description,
        string extTransactionNo,
        bool isCredit,
        string payerPayeeId,
        //string refTransactionId,
        string subCategoryId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType)
    {
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Description = description;
        ExtTransactionNo = extTransactionNo;
        IsCredit = isCredit;
        PayerPayeeId = payerPayeeId;
        //RefTransactionId = refTransactionId;
        SubCategoryId = subCategoryId;
        Tags = tags?.ToList() ?? [];
        TotalAmount = totalAmount;
        TransactionDate = transactionDate;
        TransactionType = transactionType;
    }
}

#region - Credit Transactions -

/// <summary>
/// A Credit Transaction ("CR") is a transaction that takes money from an Account.
/// </summary>
public record CreditTransactionDto : TransactionDto
{
    [JsonConstructor]
    public CreditTransactionDto() :
        base()
    {
        IsCredit = true;
    }

    protected CreditTransactionDto(TransactionEnums.TransactionKeys transactionType) :
        base(true, transactionType)
    { }

    public override decimal TotalAmount { get; set; }

    protected CreditTransactionDto(
        string userId,
        string accountId,
        string categoryId,
        string description,
        string extTransactionNo,
        string payerPayeeId,
        //string refTransactionId,
        string subCategoryId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            categoryId,
            description,
            extTransactionNo,
            isCredit: true,
            payerPayeeId,
            //refTransactionId,
            subCategoryId,
            tags,
            totalAmount,
            transactionDate,
            transactionType)
    {
        IsCredit = true;
    }
}

/// <summary>
/// Represents a transaction where money placed into the Account, from another Account (Payer).
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
        string categoryId,
        string description,
        string extTransactionNo,
        string payerPayeeId,
        //string refTransactionId,
        string subCategoryId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            categoryId,
            description,
            extTransactionNo,
            payerPayeeId,
            //refTransactionId,
            subCategoryId,
            tags,
            totalAmount,
            transactionDate,
            TransactionEnums.TransactionKeys.DEPOSIT)
    {
        IsCredit = false;
    }
}

#endregion

#region - Debit Transactions -

/// <summary>
/// A Debit Transaction ("DR") is a transaction that adds money to an Account.
/// </summary>
public record DebitTransactionDto : TransactionDto
{
    [JsonConstructor]
    public DebitTransactionDto() :
        base()
    {
        IsCredit = false;
    }

    protected DebitTransactionDto(TransactionEnums.TransactionKeys transactionType) :
        base(false, transactionType)
    { }

    protected DebitTransactionDto(
        string userId,
        string accountId,
        string categoryId,
        string description,
        string extTransactionNo,
        string payerPayeeId,
        //string refTransactionId,
        string subCategoryId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            categoryId,
            description,
            extTransactionNo,
            isCredit: false,
            payerPayeeId,
            //refTransactionId,
            subCategoryId,
            tags,
            totalAmount,
            transactionDate,
            transactionType)
    {
        IsCredit = false;
    }
}

public sealed record PurchasesTransactionDto :
    DebitTransactionDto
{
    public bool IsPaid => PaidDate.HasValue;

    public List<TransactionItemDto> Items { get; set; } = [];

    public DateTime? PaidDate { get; set; }

    public override decimal TotalAmount => Items.Sum(i => i.Amount);

    public decimal TotalOwing => TotalAmount - TotalPaid;

    public decimal TotalPaid { get; set; }

    [JsonConstructor]
    public PurchasesTransactionDto() :
        base(TransactionEnums.TransactionKeys.PURCHASE)
    { }

    public PurchasesTransactionDto(
        string userId,
        string accountId,
        string description,
        string extTransactionNo,
        IEnumerable<TransactionItemDto>? items,
        string payerPayeeId,
        //string refTransactionId,
        IEnumerable<string>? tags,
        decimal totalPaid,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            categoryId: string.Empty,
            description,
            extTransactionNo,
            payerPayeeId,
            //refTransactionId,
            subCategoryId: string.Empty,
            tags,
            totalAmount: 0,
            transactionDate,
            transactionType: TransactionEnums.TransactionKeys.PURCHASE)
    {
        Items = items?.ToList() ?? [];
        TotalPaid = totalPaid;
    }
}

/// <summary>
/// A transaction that takes money out of an Account,
/// such as a withdrawal from a checking Account.
/// </summary>
public sealed record WithdrawalTransactionDto :
    DebitTransactionDto
{
    [JsonConstructor]
    public WithdrawalTransactionDto() :
        base(TransactionEnums.TransactionKeys.WITHDRAWAL)
    { }

    /// <summary>
    /// Represents a transaction where money is taken out of the Account, and deposited into another Account (Payee).
    /// </summary>
    public WithdrawalTransactionDto(
        string userId,
        string accountId,
        string categoryId,
        string description,
        string extTransactionNo,
        string payerPayeeId,
        //string refTransactionId,
        string subCategoryId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            categoryId,
            description,
            extTransactionNo,
            payerPayeeId,
            //refTransactionId,
            subCategoryId,
            tags,
            totalAmount,
            transactionDate,
            TransactionEnums.TransactionKeys.WITHDRAWAL)
    { }
}


public abstract record PaymentTransactionDto :
    TransactionDto
{
    /// <summary>
    /// Whether the payment was made to, or from, and existing account within the system.
    /// </summary>
    public bool IsOwnAccount { get; set; }

    [JsonConstructor]
    protected PaymentTransactionDto() :
        base(
            isCredit: false,
            transactionType: TransactionEnums.TransactionKeys.PAYMENT_OUT)
    { }

    protected PaymentTransactionDto(TransactionEnums.TransactionKeys transactionType) :
        base(
            isCredit: transactionType.Equals(TransactionEnums.TransactionKeys.PAYMENT_IN),
            transactionType)
    { }

    protected PaymentTransactionDto(
        string userId,
        string accountId,
        string categoryId,
        string description,
        string extTransactionNo,
        bool isExistingAccount,
        string payerPayeeId,
        //string refTransactionId,
        string subCategoryId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate,
        TransactionEnums.TransactionKeys transactionType) :
        base(
            userId,
            accountId,
            categoryId,
            description,
            extTransactionNo,
            isCredit: transactionType.Equals(TransactionEnums.TransactionKeys.PAYMENT_IN),
            payerPayeeId,
            //refTransactionId,
            subCategoryId,
            tags,
            totalAmount,
            transactionDate,
            transactionType)
    {
        IsOwnAccount = isExistingAccount;
    }
}

public sealed record PaymentOutTransactionDto :
    PaymentTransactionDto
{
    [JsonConstructor]
    public PaymentOutTransactionDto() :
        base(TransactionEnums.TransactionKeys.PAYMENT_OUT)
    { }

    public PaymentOutTransactionDto(
        string userId,
        string accountId,
        string categoryId,
        string description,
        string extTransactionNo,
        bool isToOwnAccount,
        string paidToId,
        //string refTransactionId,
        string subCategoryId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            categoryId,
            description,
            extTransactionNo,
            isToOwnAccount,
            paidToId,
            //refTransactionId,
            subCategoryId,
            tags,
            totalAmount,
            transactionDate,
            transactionType: TransactionEnums.TransactionKeys.PAYMENT)
    { }
}

public sealed record PaymentInTransactionDto :
    PaymentTransactionDto
{
    [JsonConstructor]
    public PaymentInTransactionDto() :
        base(transactionType: TransactionEnums.TransactionKeys.PAYMENT_IN)
    { }

    public PaymentInTransactionDto(
        string userId,
        string accountId,
        string categoryId,
        string description,
        string extTransactionNo,
        bool isFromOwnAccount,
        string payerPayeeId,
        //string refTransactionId,
        string subCategoryId,
        IEnumerable<string>? tags,
        decimal totalAmount,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            categoryId,
            description,
            extTransactionNo,
            isFromOwnAccount,
            payerPayeeId,
            //refTransactionId,
            subCategoryId,
            tags,
            totalAmount,
            transactionDate,
            transactionType: TransactionEnums.TransactionKeys.PAYMENT_IN)
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