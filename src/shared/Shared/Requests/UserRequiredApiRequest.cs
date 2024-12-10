using System.ComponentModel.DataAnnotations;

namespace Habanerio.Xpnss.Shared.Requests;

public record UserRequiredApiRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;
}
