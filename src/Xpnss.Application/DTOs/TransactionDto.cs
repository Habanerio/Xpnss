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

    public List<string> Tags { get; set; } = [];

    public virtual decimal TotalAmount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string TransactionType { get; protected set; }

    private TransactionDto() { }

    [JsonConstructor]
    public TransactionDto(string id, string userId, string accountId, string description, DateTime transactionDate)
    {
        Id = id;
        UserId = userId;
        AccountId = accountId;
        Description = description;
        TransactionDate = transactionDate;
    }

    protected TransactionDto(bool isCredit, TransactionTypes.Keys transactionType)
    {
        IsCredit = isCredit;
        TransactionType = transactionType.ToString();
    }
}

#region - Credit Transactions -

public abstract record CreditTransactionDto : TransactionDto
{
    protected CreditTransactionDto(TransactionTypes.Keys transactionType) :
        base(true, transactionType)
    { }
}

public record DepositTransactionDto() :
    CreditTransactionDto(TransactionTypes.Keys.DEPOSIT)
{
    //public string CategoryId { get; set; } = "";

    public string PayerPayeeId { get; set; } = "";

    public PayerPayeeDto? PayerPayee { get; set; }
}

#endregion

#region - Debit Transactions -

public abstract record DebitTransactionDto : TransactionDto
{
    protected DebitTransactionDto(TransactionTypes.Keys transactionType) :
        base(false, transactionType)
    { }
}

public record PaymentTransactionDto() : DebitTransactionDto(TransactionTypes.Keys.PAYMENT)
{
    public string PaymentFromAccountId { get; set; } = "";

    public string PaymentToAccountId { get; set; } = "";
}

public record PurchaseTransactionDto() : DebitTransactionDto(TransactionTypes.Keys.PURCHASE)
{
    public bool IsPaid => PaidDate.HasValue;

    public List<TransactionItemDto> Items { get; set; } = [];

    public string PayerPayeeId { get; set; } = "";

    public PayerPayeeDto? PayerPayee { get; set; }

    public DateTime? PaidDate { get; set; }

    public override decimal TotalAmount => Items.Sum(i => i.Amount);

    public decimal TotalOwing => TotalAmount - TotalPaid;

    public decimal TotalPaid { get; set; }
}

public record WithdrawalTransactionDto() : DebitTransactionDto(TransactionTypes.Keys.WITHDRAWAL)
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

public sealed record TransactionItemDto
{
    public string Id { get; set; } = "";

    public string CategoryId { get; set; } = "";

    public string Description { get; set; } = "";

    public decimal Amount { get; set; }
}