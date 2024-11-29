using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.DTOs;

public record TransactionDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string AccountId { get; set; }

    public string Description { get; set; } = "";

    public bool IsCredit { get; protected set; }

    public string? PayerPayeeId { get; set; } = "";

    public PayerPayeeDto? PayerPayee { get; set; }

    public List<string> Tags { get; set; } = [];

    public virtual decimal TotalAmount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string TransactionType { get; protected set; }

    private TransactionDto() { }

    [JsonConstructor]
    public TransactionDto(
        string id,
        string userId,
        string accountId,
        string description,
        DateTime transactionDate)
    {
        Id = id;
        UserId = userId;
        AccountId = accountId;
        Description = description;
        TransactionDate = transactionDate;
    }

    protected TransactionDto(
        bool isCredit,
        TransactionTypes.Keys transactionType)
    {
        IsCredit = isCredit;
        TransactionType = transactionType.ToString();
    }
}

#region - Credit Transactions -

/// <summary>
/// A Credit Transaction ("CR") is a transaction that takes money from an account.
/// </summary>
public abstract record CreditTransactionDto : TransactionDto
{
    protected CreditTransactionDto(TransactionTypes.Keys transactionType) :
        base(true, transactionType)
    { }
}

/// <summary>
/// A payment towards money owed
/// </summary>
public sealed record CreditPaymentTransactionDto() :
    CreditTransactionDto(TransactionTypes.Keys.PAYMENT)
{
    public string PaymentFromAccountId { get; set; } = "";

    public string PaymentToAccountId { get; set; } = "";
}

public sealed record PurchaseTransactionDto() :
    CreditTransactionDto(TransactionTypes.Keys.PURCHASE)
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
    CreditTransactionDto(TransactionTypes.Keys.WITHDRAWAL)
{
    /// <summary>
    /// The Id of the underlying account that the withdrawal was made FROM.
    /// </summary>
    public string WithdrewFromAccountId { get; set; } = "";

    /// <summary>
    /// The Id of the underlying account that the withdrawal was made TO.
    /// </summary>
    public string WithdrewToAccountId { get; set; } = "";
}

#endregion

#region - Debit Transactions -

/// <summary>
/// A Debit Transaction ("DR") is a transaction that adds money to an account.
/// </summary>
public abstract record DebitTransactionDto : TransactionDto
{
    protected DebitTransactionDto(TransactionTypes.Keys transactionType) :
        base(false, transactionType)
    { }
}

/// <summary>
/// A transaction that adds money to an account,
/// such as cash to a checking account.
/// </summary>
public sealed record DepositTransactionDto() :
    DebitTransactionDto(TransactionTypes.Keys.DEPOSIT)
{
    //public string CategoryId { get; set; } = "";
}

#endregion

public sealed record TransactionItemDto
{
    public string Id { get; set; } = "";

    public string CategoryId { get; set; } = "";

    public string Description { get; set; } = "";

    public decimal Amount { get; set; }
}