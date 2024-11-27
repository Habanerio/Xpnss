using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Interfaces;

/// <summary>
/// Interface for entities that have an overdraft creditLimit.
/// </summary>
public interface IHasOverdraftAmount
{
    /// <summary>
    /// The overdraft creditLimit.
    /// </summary>
    Money OverdraftAmount { get; }

    /// <summary>
    /// Indicates if the Account is over the overdraft limit.
    /// </summary>
    bool IsOverLimit { get; }

    void UpdateOverdraftAmount(Money newOverdraftAmount);

    ///// <summary>
    ///// Adjusts the overdraft creditLimit.
    ///// </summary>
    ///// <param name="value">The new value of the overdraft creditLimit</param>
    ///// <param name="dateChanged">The date that the change went into effect.<br />
    ///// This is not the date that the user updated
    ///// </param>
    ///// <param name="reason">The reason why the change was made (optional)</param>
    //void AddOverdraftAdjustment(Money value, DateTime dateChanged, string reason = "");
}