using System.ComponentModel.DataAnnotations;

namespace Habanerio.Xpnss.Shared.Requests;

/// <summary>
/// For any API request that requires a UserId
/// </summary>
public record UserRequiredRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;
}
