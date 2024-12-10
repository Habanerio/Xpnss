using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Interfaces;

/// <summary>
/// Interface for entities that have a credit limit.
/// </summary>
public interface IHasCreditLimit
{
    /// <summary>
    /// The max creditLimit that the Account can be in debt.
    /// </summary>
    Money CreditLimit { get; set; }

    /// <summary>
    /// Indicates if the Account is over the credit limit.
    /// </summary>
    bool IsOverLimit { get; }

    void UpdateCreditLimit(Money newCreditLimit);

    ///// <summary>
    ///// Adjusts the credit limit.
    ///// This is for when their bank or credit card company increases or decreases their credit limit.
    ///// </summary>
    ///// <param name="value">The new value of the credit limit</param>
    ///// <param name="dateChanged">The date that the change went into effect.<br />
    ///// This is not the date that the user updated
    ///// </param>
    ///// <param name="reason">The reason why the change was made (optional)</param>
    //void AddCreditLimitAdjustment(Money value, DateTime dateChanged, string reason = "");
}