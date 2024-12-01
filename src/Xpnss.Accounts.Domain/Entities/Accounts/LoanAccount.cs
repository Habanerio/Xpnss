using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;

public sealed class LoanAccount : BaseCreditAccount
{
    public bool IsPaidOff => DatePaidOff.HasValue;

    /// <summary>
    /// The creditLimit of interest that has been paid on the loan since the start date,
    /// using the original creditLimit of the loan, and NOT the remaining balance.
    /// </summary>
    public decimal InterestPaid
    {
        get
        {
            TimeSpan timeElapsed = DateTime.Now.Date - DateTermStarted.Date;
            double totalDays = timeElapsed.TotalDays;

            // Interest is applied to the original creditLimit, and not the remaining balance.
            decimal interestPaid = ((decimal)(totalDays) * (InterestRate / 365) * CreditLimit.Value);
            return interestPaid;
        }
    }

    /// <summary>
    /// The number of months that the loan is for.
    /// </summary>
    public int TermMonths { get; set; }

    /// <summary>
    /// The date that the loan starts.
    /// </summary>
    public DateTime DateTermStarted { get; set; }

    public DateTime? DatePaidOff { get; set; }

    private LoanAccount(
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate) :
        base(
            userId,
            AccountTypes.Keys.LINE_OF_CREDIT,
            accountName,
            balance,
            description,
            displayColor,
            creditLimit,
            interestRate)
    {
        throw new NotImplementedException();
    }

    private LoanAccount(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        DateTime? dateClosed,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            AccountTypes.Keys.LINE_OF_CREDIT,
            accountName,
            balance,
            description,
            displayColor,
            creditLimit,
            interestRate,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates an instance of a NEW Line of Credit Account.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If `id` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `userId` is null or empty</exception>
    /// <exception cref="ArgumentNullException">If `accountName` is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `creditLimit` is below 0</exception>
    /// <exception cref="ArgumentOutOfRangeException">If `interestRate` is below 0 or above 100</exception>
    public static LoanAccount Load(
        AccountId id,
        UserId userId,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        Money creditLimit,
        PercentageRate interestRate,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated)
    {
        if (creditLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(creditLimit));

        return new LoanAccount(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,
            creditLimit,
            interestRate,
            dateClosed,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    /// <summary>
    /// Creates an instance of a NEW Line of Credit Account.
    /// </summary>
    /// <returns></returns>
    public static LoanAccount New(
        UserId userId,
        AccountName accountName,
        string description,
        string displayColor,

        Money creditLimit,
        PercentageRate interestRate)
    {
        return new LoanAccount(
            userId,
            accountName,
            Money.Zero,
            description,
            displayColor,
            creditLimit,
            interestRate);
    }
}