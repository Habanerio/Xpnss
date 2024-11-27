using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Domain.Interfaces;

/// <summary>
/// Interface for entities that have an interest rate.
/// </summary>
public interface IHasInterestRate
{
    /// <summary>
    /// The interest rate.
    /// </summary>
    PercentageRate InterestRate { get; set; }

    void UpdateInterestRate(PercentageRate newInterestRate);

    ///// <summary>
    ///// Adjusts the interest rate.
    ///// This is for when their bank or credit card company increases or decreases their interest rate.
    ///// </summary>
    ///// <param name="value">The new value of the interest rate</param>
    ///// <param name="dateChanged">The date that the change went into effect.<br />
    ///// This is not the date that the user updated
    ///// </param>
    ///// <param name="reason">The reason why the change was made (optional)</param>
    //void AddInterestRateAdjustment(PercentageRate value, DateTime dateChanged, string reason = "");
}