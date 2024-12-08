namespace Habanerio.Xpnss.Application.DTOs;

public sealed record MonthlyTotalDto
{
    public string EntityId { get; init; }

    public string SubEntityId { get; init; }

    public string EntityType { get; init; }

    public int Year { get; init; }

    public int Month { get; init; }

    public decimal CreditTotalAmount { get; init; }

    public int CreditCount { get; init; }

    public decimal DebitTotalAmount { get; init; }

    public int DebitCount { get; init; }
}