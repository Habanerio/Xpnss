namespace Habanerio.Xpnss.Modules.Common.Events;

public class TransactionAddedEvent
{
    public string UserId { get; set; }

    public string AccountId { get; set; }

    public decimal Amount { get; set; }

    public bool IsCredit { get; set; }
}