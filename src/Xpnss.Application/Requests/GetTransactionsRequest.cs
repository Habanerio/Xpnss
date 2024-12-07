using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Application.Requests;

public record GetTransactionsRequest
{
    [Required]
    public string UserId { get; set; }

    public string AccountId { get; set; }

    //[Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string TimeZone { get; set; } = "";

    [JsonConstructor]
    public GetTransactionsRequest() { }


    public GetTransactionsRequest(string userId, string accountId)
    {
        UserId = userId;
        AccountId = accountId;
    }
}