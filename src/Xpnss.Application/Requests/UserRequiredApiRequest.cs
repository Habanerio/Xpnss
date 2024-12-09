using System.ComponentModel.DataAnnotations;

namespace Habanerio.Xpnss.Application.Requests;

public record UserRequiredApiRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;
}
