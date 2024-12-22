using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

/// <summary>
/// For Purchases where there are more than one grouping of items (eg: Groceries, Cloth, Home)
/// </summary>
public class PurchasesTransaction :
    DebitTransaction
{
    private readonly List<TransactionPaymentItem> _payments = [];

    public bool IsPaid => PaidDate.HasValue;

    public DateTime? PaidDate => TotalOwing.Value <= 0 ? _payments.Max(p => p.PaymentDate).Date : null;


    public IReadOnlyCollection<TransactionPaymentItem> Payments => _payments.AsReadOnly();

    //public override Money Amount => new(_items.Sum(i => i.Amount.Value));

    public Money TotalOwing => TotalAmount - TotalPaid;

    public Money TotalPaid => new(_payments.Sum(p => p.Amount.Value));


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
    private PurchasesTransaction(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate)
        : base(
            userId,
            accountId,
            description,
            extTransactionNo,
            items,
            payerPayeeId,
            //refTransactionId,
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
    private PurchasesTransaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
        : base(
            id,
            userId,
            accountId,
            description,
            extTransactionNo,
            items,
            payerPayeeId,
            //refTransactionId,
            tags,
            transactionDate,
            TransactionEnums.TransactionKeys.PURCHASE,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        _payments = [];
    }

    public static PurchasesTransaction Load(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new PurchasesTransaction(
            id,
            userId,
            accountId,
            description,
            extTransactionNo,
            items,
            payerPayeeId,
            //refTransactionId,
            tags,
            transactionDate,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    public static PurchasesTransaction New(
        UserId userId,
        AccountId accountId,
        string description,
        string extTransactionNo,
        IEnumerable<TransactionItem> items,
        PayerPayeeId payerPayeeId,
        //RefTransactionId refTransactionId,
        IEnumerable<string>? tags,
        DateTime transactionDate)
    {
        return new PurchasesTransaction(
            userId,
            accountId,
            description,
            extTransactionNo,
            items,
            payerPayeeId,
            //refTransactionId,
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
        // AddDomainEvent(new TransactionPaidEvent(Id, userId, Amount, paymentDate));

        return remaining;
    }


    private void ApplyPayment(Money paymentAmount, DateTime paymentDate)
    {
        _payments.Add(TransactionPaymentItem.New(paymentAmount, paymentDate));

        DateUpdated = DateTime.UtcNow;
    }
}