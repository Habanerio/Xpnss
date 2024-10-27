namespace Habanerio.Xpnss.Modules.Accounts.DTOs;

public sealed record MonthlyTotalsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Total { get; set; }
    public int TransactionCount { get; set; }
}