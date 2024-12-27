using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Shared.Requests.Transactions;

public record CreateTransactionRequest : UserRequiredRequest
{
    private DateTime _transactionDate;

    [Required]
    public string AccountId { get; init; } = string.Empty;

    public string CategoryId { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ExtTransactionNo { get; init; } = string.Empty;

    public bool IsCredit { get; init; }

    public PayerPayeeRequest PayerPayee { get; init; } = new();

    public string RefTransactionId { get; set; } = string.Empty;

    public string SubCategoryId { get; init; } = string.Empty;

    public List<string> Tags { get; init; } = [];

    public string Title { get; init; } = string.Empty;

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
        decimal totalAmount,
        string categoryId,
        string description,
        string extTransactionNo,
        bool isCredit,
        PayerPayeeRequest payerPayee,
        string refTransactionId,
        string subCategoryId,
        IEnumerable<string>? tags,
        string title,
        DateTime transactionDate)
    {
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        TotalAmount = totalAmount;
        Description = description;
        IsCredit = isCredit;
        ExtTransactionNo = extTransactionNo;
        PayerPayee = payerPayee;
        RefTransactionId = refTransactionId;
        SubCategoryId = subCategoryId;
        Tags = tags?.ToList() ?? [];
        Title = title;
        TransactionDate = transactionDate;
        TransactionType = transactionType;
    }
}

#region - Credit Transactions -

public record CreateCreditTransactionRequest :
    CreateTransactionRequest
{
    [JsonConstructor]
    public CreateCreditTransactionRequest() :
        base()
    {
        IsCredit = true;
    }

    public CreateCreditTransactionRequest(TransactionEnums.TransactionKeys transactionType) :
        base(true, transactionType)
    { }

    public CreateCreditTransactionRequest(
        string userId,
        TransactionEnums.TransactionKeys transactionType,
        string accountId,
        decimal amount,
        string categoryId,
        string description,
        PayerPayeeRequest payerPayee,
        string subCategoryId,
        string title,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionNo = "",
        string refTransactionId = "") :
        base(
            userId,
            transactionType,
            accountId,
            amount,
            categoryId,
            description,
            extTransactionNo,
            isCredit: true,
            payerPayee,
            refTransactionId,
            subCategoryId,
            tags,
            title,
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
        string categoryId,
        string description,
        PayerPayeeRequest depositFrom,
        string subCategoryId,
        string title,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionNo = "",
        string refTransactionId = "") :
        base(
            userId,
            transactionType: TransactionEnums.TransactionKeys.DEPOSIT,
            accountId,
            amount,
            categoryId,
            description,
            depositFrom,
            subCategoryId,
            title,
            transactionDate,
            tags,
            extTransactionNo,
            refTransactionId)
    { }
}


/// <summary>
/// For payments where you receive (credit) money into your account.
/// If this is from an existing account, then only create a `Payment Made` Request.
/// </summary>
public sealed record CreatePaymentInTransactionRequest :
    CreateCreditTransactionRequest
{
    public bool IsFromOwnAccount { get; set; }

    [JsonConstructor]
    public CreatePaymentInTransactionRequest() :
        base(TransactionEnums.TransactionKeys.PAYMENT_IN)
    { }

    public CreatePaymentInTransactionRequest(
        string userId,
        string accountId,
        decimal amount,
        string categoryId,
        string description,
        bool isPaidTFromOwnAccount,
        PayerPayeeRequest paymentInFrom,
        string subCategoryId,
        string title,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionNo = "",
        string refTransactionId = "") :
        base(
            userId,
            transactionType: TransactionEnums.TransactionKeys.PAYMENT_IN,
            accountId,
            amount,
            categoryId,
            description,
            payerPayee: paymentInFrom,
            subCategoryId,
            title,
            transactionDate,
            tags,
            extTransactionNo,
            refTransactionId)
    {
        IsFromOwnAccount = isPaidTFromOwnAccount;
    }
}

#endregion // Credit Transactions



#region - Debit Transactions -

public record CreateDebitTransactionRequest :
    CreateTransactionRequest
{
    [JsonConstructor]
    public CreateDebitTransactionRequest() :
        base()
    {
        IsCredit = false;
    }

    public CreateDebitTransactionRequest(TransactionEnums.TransactionKeys transactionType) :
        base(false, transactionType)
    { }

    public CreateDebitTransactionRequest(
        string userId,
        TransactionEnums.TransactionKeys transactionType,
        string accountId,
        decimal totalAmount,
        string categoryId,
        string description,
        PayerPayeeRequest payerPayee,
        string subCategoryId,
        string title,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionNo = "",
        string refTransactionId = "") :
        base(
            userId,
            transactionType,
            accountId,
            totalAmount,
            categoryId,
            description,
            extTransactionNo,
            isCredit: false,
            payerPayee,
            refTransactionId,
            subCategoryId,
            tags,
            title,
            transactionDate)
    { }
}


/// <summary>
/// For payments where you pay (debit) money out of your account.
/// If it's a payment to an existing account, set IsExistingAccount to true,
/// and only use this one. Do NOT create a Payment Received Request
/// </summary>
public sealed record CreatePaymentOutTransactionRequest :
    CreateDebitTransactionRequest
{
    public bool IsToOwnAccount { get; set; }

    [JsonConstructor]
    public CreatePaymentOutTransactionRequest() :
        base(TransactionEnums.TransactionKeys.PAYMENT_OUT)
    { }

    public CreatePaymentOutTransactionRequest(
        string userId,
        string accountId,
        decimal totalAmount,
        string categoryId,
        string description,
        PayerPayeeRequest paymentOutTo,
        bool isPaidToOwnAccount,
        string subCategoryId,
        string title,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionNo = "",
        string refTransactionId = "") :
        base(
            userId,
            transactionType: TransactionEnums.TransactionKeys.PAYMENT_OUT,
            accountId,
            totalAmount,
            categoryId,
            description,
            payerPayee: paymentOutTo,
            subCategoryId,
            title,
            transactionDate,
            tags,
            extTransactionNo,
            refTransactionId)
    {
        IsToOwnAccount = isPaidToOwnAccount;
    }
}


/// <summary>
/// For when a purchase has more than one groupings of items (split)
/// </summary>
public sealed record CreatePurchasesTransactionRequest :
    CreateDebitTransactionRequest
{
    public List<TransactionRequestItem> Items { get; set; } = [];

    public override decimal TotalAmount => Items.Sum(i => i.Amount);

    [JsonConstructor]
    public CreatePurchasesTransactionRequest() :
        base(TransactionEnums.TransactionKeys.PURCHASE)
    { }

    public CreatePurchasesTransactionRequest(
        string userId,
        string accountId,
        PayerPayeeRequest payee,
        string description,
        DateTime transactionDate,
        IEnumerable<TransactionRequestItem> items,
        string title,
        IEnumerable<string>? tags = null,
        string extTransactionNo = "",
        string refTransactionId = "") :
        base(TransactionEnums.TransactionKeys.PURCHASE)
    {
        UserId = userId;
        AccountId = accountId;
        PayerPayee = payee;
        Description = description;
        ExtTransactionNo = extTransactionNo;
        TransactionDate = transactionDate;
        Items = items?.ToList() ?? [];
        RefTransactionId = refTransactionId;
        Tags = tags?.ToList() ?? [];
        Title = title;
        TotalAmount = Items.Sum(i => i.Amount);
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
        string userId,
        string accountId,
        decimal totalAmount,
        string categoryId,
        string description,
        PayerPayeeRequest withdrewTo,
        string subCategoryId,
        string title,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionNo = "",
        string refTransactionId = "") :
        base(
            userId,
            TransactionEnums.TransactionKeys.WITHDRAWAL,
            accountId,
            totalAmount,
            categoryId,
            description,
            withdrewTo,
            subCategoryId,
            title,
            transactionDate,
            tags,
            extTransactionNo,
            refTransactionId)
    {
        //if (string.IsNullOrWhiteSpace(cashAccountId))
        //    throw new ArgumentException("Cash Account Id is required for Withdrawal Transactions. " +
        //        "The money has to go somewhere");
    }
}

#endregion // Debit Transactions


public sealed record TransactionRequestItem
{
    public decimal Amount { get; set; }

    public string CategoryId { get; set; } = string.Empty;

    public string SubCategoryId { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [JsonConstructor]
    public TransactionRequestItem() { }

    public TransactionRequestItem(
        decimal amount,
        string categoryId,
        string subCategoryId,
        string description)
    {
        Amount = amount;
        CategoryId = categoryId;
        SubCategoryId = subCategoryId;
        Description = description;
    }
}