using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Accounts.Interfaces;

public interface IHasInterestRate
{
    PercentageRate InterestRate { get; set; }

    void AdjustInterestRate(PercentageRate newValue, DateTime dateChanged, string reason = "");
}