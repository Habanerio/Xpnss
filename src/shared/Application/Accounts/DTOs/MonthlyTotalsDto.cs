namespace Habanerio.Xpnss.Application.Accounts.DTOs;

public sealed record MonthlyTotalsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal CreditTotalAmount { get; set; }
    public int CreditCount { get; set; }
    public decimal DebitTotalAmount { get; set; }
    public int DebitCount { get; set; }
}