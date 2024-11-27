using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities;

public sealed class TransactionPayment : Entity<TransactionPaymentId>
{
    public Money Amount { get; init; }

    public DateTime PaymentDate { get; init; }

    /// <summary>
    /// For NEW Payments
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="dateOfPayment"></param>
    private TransactionPayment(Money amount, DateTime dateOfPayment) :
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
    private TransactionPayment(TransactionPaymentId id, Money amount, DateTime dateOfPayment) : base(id)
    {
        Amount = amount;
        PaymentDate = dateOfPayment;
    }

    public static TransactionPayment Load(TransactionPaymentId id, decimal amount, DateTime dateOfPayment)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException($"{nameof(id)} cannot be null or whitespace.", nameof(id));

        if (amount < 0)
            throw new ArgumentException($"{nameof(amount)} cannot be less than 0.", nameof(amount));

        return new TransactionPayment(id, new Money(amount), dateOfPayment);
    }

    public static TransactionPayment New(Money amount, DateTime dateOfPayment)
    {
        return new TransactionPayment(amount, dateOfPayment);
    }
}