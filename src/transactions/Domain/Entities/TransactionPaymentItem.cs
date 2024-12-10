using Habanerio.Xpnss.Shared.Entities;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities;

/// <summary>
/// Represents a Payment made for a Transaction.
/// </summary>
public sealed class TransactionPaymentItem : Entity<TransactionPaymentId>
{
    public Money Amount { get; init; }

    public DateTime PaymentDate { get; init; }

    /// <summary>
    /// For NEW Payments
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="dateOfPayment"></param>
    private TransactionPaymentItem(Money amount, DateTime dateOfPayment) :
        this(TransactionPaymentId.New, amount, dateOfPayment)
    {
        IsTransient = true;
    }

    /// <summary>
    /// For EXISTING Payments
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amount"></param>
    /// <param name="dateOfPayment"></param>
    private TransactionPaymentItem(
        TransactionPaymentId id,
        Money amount,
        DateTime dateOfPayment) : base(id)
    {
        Amount = amount;
        PaymentDate = dateOfPayment;
    }

    public static TransactionPaymentItem Load(
        TransactionPaymentId id,
        decimal amount,
        DateTime dateOfPayment)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException($"{nameof(id)} cannot be null or whitespace.", nameof(id));

        if (amount < 0)
            throw new ArgumentException($"{nameof(amount)} cannot be less than 0.", nameof(amount));

        return new TransactionPaymentItem(id, new Money(amount), dateOfPayment);
    }

    public static TransactionPaymentItem New(Money amount, DateTime dateOfPayment)
    {
        return new TransactionPaymentItem(amount, dateOfPayment);
    }
}