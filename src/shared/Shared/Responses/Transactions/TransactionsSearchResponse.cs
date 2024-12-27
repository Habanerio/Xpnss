using Habanerio.Xpnss.Shared.DTOs.Transactions;

namespace Habanerio.Xpnss.Shared.Responses.Transactions;

public record TransactionsSearchResponse
{
    public string UserId { get; set; } = string.Empty;

    public KeyValuePair<string, string>? ForAccount { get; set; }

    public KeyValuePair<string, string>? ForCategory { get; set; }

    public KeyValuePair<string, string>? ForPayerPayee { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int PageNo { get; set; } = 1;

    public int PerPage { get; set; } = 100;

    public string TimeZome { get; set; } = string.Empty;

    public int TotalPages { get; set; }

    public int TotalResults { get; set; }

    public IEnumerable<TransactionLineItemDto> Transactions { get; set; } = [];
}