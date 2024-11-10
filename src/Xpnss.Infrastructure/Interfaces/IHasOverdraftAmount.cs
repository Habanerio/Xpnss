namespace Habanerio.Xpnss.Infrastructure.Interfaces;

internal interface IHasOverdraftAmount
{
    decimal OverDraftAmount { get; set; }
}