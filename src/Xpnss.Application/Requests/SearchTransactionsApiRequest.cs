using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Application.Requests;

public record SearchTransactionsApiRequest
{
    [Required]
    public string UserId { get; set; }

    public string AccountId { get; set; }

    public string CategoryId { get; set; }

    //[Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string TimeZone { get; set; } = string.Empty;

    [JsonConstructor]
    public SearchTransactionsApiRequest() { }


    public SearchTransactionsApiRequest(string userId, string accountId)
    {
        UserId = userId;
        AccountId = accountId;
    }
}