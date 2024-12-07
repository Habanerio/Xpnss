using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public class PurchaseTransaction :
    DebitTransaction
{
    private readonly List<TransactionItem> _items;
    private readonly List<TransactionPaymentItem> _payments;

    private const int MAX_ITEMS_PER_TRANSACTION = 25;

    public bool IsPaid => DatePaid.HasValue;

    public IReadOnlyCollection<TransactionItem> Items => _items.AsReadOnly();

    public IReadOnlyCollection<TransactionPaymentItem> Payments => _payments.AsReadOnly();

    public override Money TotalAmount => new(_items.Sum(i => i.Amount.Value));

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
        PayerPayeeId payerPayeeId,
        List<TransactionItem> items,
        DateTime transactionDate,
        IEnumerable<string>? tags = null,
        string extTransactionId = "")
        : base(
            userId,
            accountId,
            TransactionEnums.TransactionKeys.PURCHASE,
            payerPayeeId,
            new Money(items.Sum(i => i.Amount)),
            description,
            transactionDate,
            tags,
            extTransactionId)
    {
        _items = items.Any() ? items.ToList() :
            throw new ArgumentException($"Purchase Transaction must have at least one Item");

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
        List<TransactionItem> items,
        List<TransactionPaymentItem> payments,
        DateTime transactionDate,
        IEnumerable<string>? tags,
        string extTransactionId,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null)
        : base(
            id,
            userId,
            accountId,
            TransactionEnums.TransactionKeys.PURCHASE,
            payerPayeeId,
            new Money(items.Sum(i => i.Amount)),
            description,
            transactionDate,
            tags,
            extTransactionId,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        _items = items.Any() ? items.ToList() :
            throw new ArgumentException($"Purchase Transaction must have at least one Item");

        _payments = payments?.ToList() ?? [];
    }

    public static PurchaseTransaction Load(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        string description,
        PayerPayeeId payerPayeeId,
        IEnumerable<TransactionItem> items,
        IEnumerable<TransactionPaymentItem> payments,
        DateTime transactionDate,
        List<string>? tags,
        string extTransactionId,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null)
    {
        return new PurchaseTransaction(
            id,
            userId,
            accountId,
            description,
            payerPayeeId,
            items.ToList(),
            payments.ToList(),
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
        PayerPayeeId payerPayeeId,
        IEnumerable<TransactionItem> items,
        DateTime transactionDate,
        IEnumerable<string>? tags)
    {
        var itemsArray = items?.ToList() ?? [];

        if (!itemsArray.Any())
            throw new ArgumentException("The entities cannot be empty", nameof(items));

        return new PurchaseTransaction(
            userId,
            accountId,
            description,
            payerPayeeId,
            itemsArray,
            transactionDate,
            tags);
    }

    public void AddItem(Money amount, CategoryId categoryId, SubCategoryId subCategoryId, string description)
    {
        if (Items.Count >= MAX_ITEMS_PER_TRANSACTION)
            throw new InvalidOperationException($"Cannot add more than {MAX_ITEMS_PER_TRANSACTION} items to a transaction.");

        if (IsDeleted)
            throw new InvalidOperationException("Cannot add items to a deleted transaction.");

        _items.Add(TransactionItem.New(amount, categoryId, subCategoryId, description));

        // AddDomainEvent(new TransactionUpdatedDomainEvent(Id, TotalAmount, TotalOwing, TotalPaid));

        // AddDomainEvent(new TransactionItemAddedDomainEvent(Id, amount, categoryId, description));
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

//public class BalanceTransferTransaction : TransactionBase
//{
//    public PercentageRate InterestRate { get; }

//    public DateTime TermEnds { get; }

//    public Money TransferFee { get; }

//    private BalanceTransferTransaction(
//        TransactionId id,
//        UserId userId,
//        AccountId accountId,
//        string description,
//        PayerPayeeId accountTransferredToId,
//        IEnumerable<TransactionItem> transactionItems,
//        DateTime? dateTermEnds,
//        PercentageRate interestRate,
//        Money transferFee,
//        DateTime dateOfTransaction,
//        DateTime dateCreated,
//        DateTime? dateUpdated = null,
//        DateTime? dateDeleted = null) :
//        base(
//            id,
//            userId,
//            accountId,
//            TransactionEnums.AccountEnums.CurrencyKeys.BALANCE_TRANSFER,
//            description,
//            accountTransferredToId,
//            transactionItems,
//            dateOfTransaction,
//            dateCreated,
//            dateUpdated,
//            dateDeleted)
//    {
//        InterestRate = interestRate;
//        TermEnds = dateTermEnds ?? DateTime.MaxValue;
//        TransferFee = transferFee;
//    }

//    /// <summary>
//    /// Loads up an existing Balance Transfer Transaction.
//    /// </summary>
//    /// <param name="id"></param>
//    /// <param name="userId">The id of the user who is transferring the balance</param>
//    /// <param name="accountId">The id of the account that the transfer is coming from</param>
//    /// <param name="accountTransferredToId">The id of the account that the transfer is going to</param>
//    /// <param name="description">Some random note about this balance transfer</param>
//    /// <param name="transactionItems">This should be a single item</param>
//    /// <param name="dateTermEnds">When does the term end? If no DateTime is provided, then it's assumed that this is not a promotion, and applies until paid off.</param>
//    /// <param name="interestRate">How much is the interest rate for this transfer? For promos, it'll be lower than the accounts %. However, some institutions charge more % for regular transfers</param>
//    /// <param name="transferFee">Was there a fee charged for this transaction? This is for record keeping only (for now). A Balance Transfer Fee Transaction will need to be created.</param>
//    /// <param name="dateOfTransaction">When the transaction happened</param>
//    /// <param name="dateCreated">When the record was added to the db</param>
//    /// <param name="dateUpdated">When was the last time it was updated?</param>
//    /// <param name="dateDeleted">When was it deleted?</param>
//    /// <param name="datePaid"></param>
//    /// <returns></returns>
//    public static BalanceTransferTransaction Load(
//        TransactionId id,
//        UserId userId,
//        AccountId accountId,
//        string description,
//        PayerPayeeId accountTransferredToId,
//        IEnumerable<TransactionItem> transactionItems,
//        DateTime? dateTermEnds,
//        PercentageRate interestRate,
//        Money transferFee,
//        DateTime dateOfTransaction,
//        DateTime dateCreated,
//        DateTime? dateUpdated = null,
//        DateTime? dateDeleted = null)
//    {
//        return new BalanceTransferTransaction(
//            id,
//            userId,
//            accountId,
//            description,
//            accountTransferredToId,
//            transactionItems,
//            dateTermEnds,
//            interestRate,
//            transferFee,
//            dateOfTransaction,
//            dateCreated,
//            dateUpdated,
//            dateDeleted);
//    }

//    /// <summary>
//    /// Creates a new Balance Transfer Transaction.
//    /// </summary>
//    /// <param name="userId">The id of the user who is transferring the balance</param>
//    /// <param name="accountId">The id of the account that the transfer is coming from</param>
//    /// <param name="accountTransferredToId">The id of the account that the transfer is going to</param>
//    /// <param name="description">Some random note about this balance transfer</param>
//    /// <param name="amount">How much was the transfer for?</param>
//    /// <param name="dateTermEnds">When does the promotion end?</param>
//    /// <param name="interestRate"></param>
//    /// <param name="transferFee"></param>
//    /// <param name="dateOfTransaction"></param>
//    /// <returns></returns>
//    public static BalanceTransferTransaction New(
//        UserId userId,
//        AccountId accountId,
//        PayerPayeeId accountTransferredToId,
//        string description,
//        Money amount,
//        DateTime? dateTermEnds,
//        PercentageRate interestRate,
//        Money transferFee,
//        DateTime dateOfTransaction)
//    {
//        var transactionItems = new List<TransactionItem>
//        {
//            TransactionItem.New(amount, CategoryId.New, description)
//        };

//        return new BalanceTransferTransaction(
//            TransactionId.New,
//            userId,
//            accountId,
//            description,
//            accountTransferredToId,
//            transactionItems,
//            dateTermEnds,
//            interestRate,
//            transferFee,
//            dateOfTransaction.Date, //.ToUniversalTime().Date,
//            DateTime.UtcNow);
//    }
//}

