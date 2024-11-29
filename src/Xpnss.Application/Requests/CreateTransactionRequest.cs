using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.Requests;

public abstract record CreateTransactionRequest
{
    public string UserId { get; init; } = string.Empty;

    public string AccountId { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public bool IsCredit { get; init; } = false;

    public PayerPayeeRequest PayerPayee { get; init; } = new();

    public List<string> Tags { get; init; } = new();

    public virtual decimal TotalAmount { get; init; }

    public DateTime TransactionDate { get; init; }

    public TransactionTypes.Keys TransactionType { get; private set; }

    protected CreateTransactionRequest(bool isCredit, TransactionTypes.Keys transactionType)
    {
        IsCredit = isCredit;
        TransactionType = transactionType;
    }
}

#region - Credit Transactions -

public abstract record CreateCreditTransactionRequest :
    CreateTransactionRequest
{
    protected CreateCreditTransactionRequest(TransactionTypes.Keys transactionType) :
        base(true, transactionType)
    { }
}

/// <summary>
/// Represents when the user deposits money into their account from an external source (eg: Income, Gift, etc)
/// </summary>
public sealed record CreateDepositTransactionRequest() :
    CreateCreditTransactionRequest(TransactionTypes.Keys.DEPOSIT)
{

}


public sealed record CreatePaymentTransactionRequest() :
    CreateDebitTransactionRequest(TransactionTypes.Keys.PAYMENT)
{

}

/// <summary>
/// For refunds where there is no purchase transaction to refund against
/// </summary>
public sealed record CreateRefundTransactionRequest() :
    CreateCreditTransactionRequest(TransactionTypes.Keys.REFUND)
{
    public string CategoryId { get; init; } = "";
}

/// <summary>
/// For refunds where there is a purchase transaction to refund against
/// </summary>
public sealed record CreateRefundPurchaseTransactionRequest() :
    CreateCreditTransactionRequest(TransactionTypes.Keys.REFUND)
{
    public string PurchaseTransactionId { get; init; } = "";

    public List<PurchaseTransactionItemRequest> Items { get; init; } = [];
}

#endregion // Credit Transactions

#region - Debit Transactions -

public abstract record CreateDebitTransactionRequest :
    CreateTransactionRequest
{
    protected CreateDebitTransactionRequest(TransactionTypes.Keys transactionType) :
        base(false, transactionType)
    { }
}

public sealed record CreatePurchaseTransactionRequest() :
    CreateDebitTransactionRequest(TransactionTypes.Keys.PURCHASE)
{
    public bool IsPaid => PaidDate.HasValue;

    public List<PurchaseTransactionItemRequest> Items { get; init; } = [];

    public DateTime? PaidDate { get; init; }

    /// <summary>
    /// The Id of the underlying account that the purchase was made with.
    /// </summary>
    public string PaidWithAccountId { get; set; } = "";

    public override decimal TotalAmount => Items.Sum(i => i.Amount);

    public decimal TotalOwing => TotalAmount - TotalPaid;

    public decimal TotalPaid { get; init; }
}

/// <summary>
/// Usually for when then money is taken out as Cash
/// </summary>
public sealed record CreateWithdrawalTransactionRequest() : CreateDebitTransactionRequest(TransactionTypes.Keys.WITHDRAWAL)
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

#endregion // Debit Transactions

/// <summary>
/// For when the balance transfer has a cost to it, such as transferring the balance from one account to another
/// </summary>
public sealed record CreateBalanceTransferRequest() :
    CreateTransactionRequest(true, TransactionTypes.Keys.BALANCE_TRANSFER)
{
    public string FromAccountId { get; init; } = "";

    public string ToAccountId { get; init; } = "";

    public decimal TransferFee { get; init; }

    public decimal TransferInterestRate { get; init; }

    public DateTime TransferToDate { get; init; } = DateTime.MaxValue;
}

/// <summary>
/// For when the Account makes a payment to another account such as Checking to Savings
/// </summary>
public sealed record CreateTransferTransactionRequest() :
    CreateCreditTransactionRequest(TransactionTypes.Keys.TRANSFER)
{
    public string FromAccountId { get; set; } = "";

    public string ToAccountId { get; set; } = "";
}


public sealed record PurchaseTransactionItemRequest
{
    public string CategoryId { get; init; } = "";

    public string Description { get; init; } = "";

    public decimal Amount { get; init; }
}