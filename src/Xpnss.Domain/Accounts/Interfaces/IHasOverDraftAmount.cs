using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Accounts.Interfaces;

public interface IHasOverDraftAmount
{
    Money OverDraftAmount { get; }

    bool IsOverLimit { get; }

    void AdjustOverDraftAmount(Money newValue, DateTime dateChanged, string reason = "");
}