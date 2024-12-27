using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Shared.Requests.Transactions;

public record SearchTransactionsRequest
{
    [Required]
    public string UserId { get; set; }

    public string AccountId { get; set; }

    public string CategoryId { get; set; }

    public string PayerPayeeId { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int PageNo { get; set; } = 1;

    public int PerPage { get; set; } = 100;

    public string TimeZone { get; set; } = string.Empty;

    [JsonConstructor]
    public SearchTransactionsRequest() { }
}