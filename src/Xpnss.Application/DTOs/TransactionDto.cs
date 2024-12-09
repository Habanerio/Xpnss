using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.DTOs;

public record TransactionDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string AccountId { get; set; }

    public string Description { get; set; } = string.Empty;

    public string ExtTransactionId { get; set; } = string.Empty;

    public bool IsCredit { get; protected set; }

    public string? PayerPayeeId { get; set; } = string.Empty;

    public PayerPayeeDto? PayerPayee { get; set; }

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
}

#region - Credit Transactions -

/// <summary>
/// A Credit Transaction ("CR") is a transaction that takes money from an account.
/// </summary>
public abstract record CreditTransactionDto : TransactionDto
{
    protected CreditTransactionDto(TransactionEnums.TransactionKeys transactionType) :
        base(true, transactionType)
    { }
}

/// <summary>
/// A transaction that adds money to an account,
/// such as cash to a checking account.
/// </summary>
public sealed record DepositTransactionDto() :
    CreditTransactionDto(TransactionEnums.TransactionKeys.DEPOSIT)
{
    //public string CategoryId { get; set; } = string.Empty;
}

#endregion

#region - Debit Transactions -

/// <summary>
/// A Debit Transaction ("DR") is a transaction that adds money to an account.
/// </summary>
public abstract record DebitTransactionDto : TransactionDto
{
    protected DebitTransactionDto(TransactionEnums.TransactionKeys transactionType) :
        base(false, transactionType)
    { }
}

public sealed record PurchaseTransactionDto() :
    DebitTransactionDto(TransactionEnums.TransactionKeys.PURCHASE)
{
    public bool IsPaid => PaidDate.HasValue;

    public List<TransactionItemDto> Items { get; set; } = [];

    public DateTime? PaidDate { get; set; }

    public override decimal TotalAmount => Items.Sum(i => i.Amount);

    public decimal TotalOwing => TotalAmount - TotalPaid;

    public decimal TotalPaid { get; set; }
}

/// <summary>
/// A transaction that takes money out of an account,
/// such as a withdrawal from a checking account.
/// </summary>
public sealed record WithdrawalTransactionDto() :
    CreditTransactionDto(TransactionEnums.TransactionKeys.WITHDRAWAL)
{
    /// <summary>
    /// The Id of the underlying account that the withdrawal was made FROM.
    /// </summary>
    public string WithdrewFromAccountId { get; set; } = string.Empty;

    /// <summary>
    /// The Id of the underlying account that the withdrawal was made TO.
    /// </summary>
    public string WithdrewToAccountId { get; set; } = string.Empty;
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