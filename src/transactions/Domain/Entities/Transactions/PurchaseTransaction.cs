using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public class PurchaseTransaction :
    DebitTransaction
{
    private readonly List<TransactionPaymentItem> _payments = [];

    public bool IsPaid => DatePaid.HasValue;

    public IReadOnlyCollection<TransactionPaymentItem> Payments => _payments.AsReadOnly();

    //public override Money TotalAmount => new(_items.Sum(i => i.Amount.Value));

    public Money TotalOwing => TotalAmount - TotalPaid;

    public Money TotalPaid => new(_payments.Sum(p => p.Amount.Value));

    public DateTime? DatePaid => TotalOwing.Value <= 0 ? _payments.Max(p => p.PaymentDate).Date : null;

    public override TransactionStatus Status
    {
        get
        {
            if (TotalOwing.Value <= 0)
                return TransactionStatus.PAID;

            return base.Status;
        }
    }

    /// <summary>
    /// For NEW (non-existing) Purchase Transactions.
    /// This adds a `TransactionCreatedDomainEvent` to the domain events.
    /// </summary>
    private PurchaseTransaction(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionId,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        IEnumerable<string>? tags,
        DateTime transactionDate)
        : base(
            userId,
            accountId,
            description,
            extTransactionId,
            items,
            payerPayeeId,
            tags,
            transactionDate,
            TransactionEnums.TransactionKeys.PURCHASE)
    {
        _payments = [];
    }

    /// <summary>
    /// For EXISTING Transactions.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private PurchaseTransaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        PayerPayeeId payerPayeeId,
        IEnumerable<TransactionItem> items,
        DateTime transactionDate,
        IEnumerable<string>? tags,
        string extTransactionId,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
        : base(
            id,
            userId,
            accountId,
            description,
            extTransactionId,
            items,
            payerPayeeId,
            tags,
            transactionDate,
            TransactionEnums.TransactionKeys.PURCHASE,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        _payments = [];
    }

    public static PurchaseTransaction Load(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        PayerPayeeId payerPayeeId,
        IEnumerable<TransactionItem> items,
        DateTime transactionDate,
        IEnumerable<string>? tags,
        string extTransactionId,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new PurchaseTransaction(
            id,
            userId,
            accountId,
            description,
            payerPayeeId,
            items,
            transactionDate,
            tags,
            extTransactionId,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    public static PurchaseTransaction New(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionId,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        IEnumerable<string>? tags,
        DateTime transactionDate)
    {
        return new PurchaseTransaction(
            userId,
            accountId,
            description,
            extTransactionId,
            items,
            payerPayeeId,
            tags,
            transactionDate);
    }

    public void AddItem(Money amount, CategoryId categoryId, SubCategoryId subCategoryId, string description)
    {
        AddTransactionItem(amount, categoryId, subCategoryId, description);
    }

    public Money AddPayment(UserId userId, Money amount, DateTime paymentDate)
    {
        if (amount < 0)
            throw new ArgumentException($"{nameof(amount)} cannot be less than 0.", nameof(amount));

        var paymentToApply = amount > TotalOwing ? TotalOwing : amount;
        var remaining = amount > TotalOwing ? amount - TotalOwing : new Money(0);

        ApplyPayment(paymentToApply, paymentDate);

        // AddDomainEvent(new TransactionPaymentAddedEvent(Id, paymentToApply, paymentDate));

        //if(TotalOwing <= 0)
        // AddDomainEvent(new TransactionPaidEvent(Id, userId, TotalAmount, paymentDate));

        return remaining;
    }


    private void ApplyPayment(Money paymentAmount, DateTime paymentDate)
    {
        _payments.Add(TransactionPaymentItem.New(paymentAmount, paymentDate));

        DateUpdated = DateTime.UtcNow;
    }
}