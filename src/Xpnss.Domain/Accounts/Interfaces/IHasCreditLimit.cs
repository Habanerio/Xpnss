using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Accounts.Interfaces;

public interface IHasCreditLimit
{
    Money CreditLimit { get; set; }

    bool IsOverLimit { get; }

    void AdjustCreditLimit(Money newValue, DateTime dateChanged, string reason = "");
}