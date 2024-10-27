namespace Habanerio.Xpnss.Modules.Transactions.DTOs;

public sealed record TransactionDto
{
    public string Id { get; init; }

    public string UserId { get; init; }

    public string AccountId { get; init; }

    public string TransactionType { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = "";

    public MerchantDto? Merchant { get; init; }

    public bool IsCredit { get; init; }

    public bool IsPaid => Paid >= Amount;

    public IReadOnlyCollection<TransactionItemDto> Items { get; init; } = [];

    public decimal Owing => Amount - Paid;

    public decimal Paid { get; init; }

    public DateTimeOffset TransactionDate { get; init; }

    public DateTimeOffset? DatePaid { get; init; }
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

    //public decimal Owing => Amount - Paid;
}

public sealed record MerchantDto
{
    public string Id { get; init; }

    public string Name { get; init; } = "";

    public string Location { get; init; } = "";
}