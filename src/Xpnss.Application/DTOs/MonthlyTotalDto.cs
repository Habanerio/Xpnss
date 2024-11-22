namespace Habanerio.Xpnss.Application.DTOs;

public record MonthlyTotalDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal CreditTotalAmount { get; set; }
    public int CreditCount { get; set; }
    public decimal DebitTotalAmount { get; set; }
    public int DebitCount { get; set; }
}

public record AccountMonthlyTotalDto : MonthlyTotalDto
{
    public string AccountId { get; set; }
}

public record CategoryMonthlyTotalDto : MonthlyTotalDto
{
    public string CategoryId { get; set; }
}

public record MerchantMonthlyTotalDto : MonthlyTotalDto
{
    public string MerchantId { get; set; }
}