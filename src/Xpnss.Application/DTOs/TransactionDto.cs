namespace Habanerio.Xpnss.Application.DTOs;

public sealed record TransactionDto
{
    public string Id { get; init; }

    public string UserId { get; init; }

    public string AccountId { get; init; }

    public string TransactionType { get; init; }

    public decimal TotalAmount { get; init; }

    public string Description { get; init; } = "";

    public string? MerchantId { get; init; } = null;

    public string? MerchantName { get; set; } = null;

    public string? MerchantLocation { get; set; } = null;

    public bool IsCredit { get; init; }

    public bool IsPaid => Paid >= TotalAmount;

    public IReadOnlyCollection<TransactionItemDto> Items { get; init; } = [];

    public decimal Owing => TotalAmount - Paid;

    public decimal Paid { get; init; }

    public DateTime TransactionDate { get; init; }

    public DateTime? DatePaid { get; init; }
}

public sealed record TransactionItemDto
{
    public string Id { get; init; }

    //public string TransactionId { get; init; }

    //public string ItemId { get; init; }

    public string CategoryId { get; init; }

    public string Description { get; init; }

    public decimal Amount { get; init; }

    //public decimal Paid { get; init; }

    //public decimal Owing => TotalAmount - Paid;
}