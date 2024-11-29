namespace Habanerio.Xpnss.Application.DTOs;

public sealed record CategoryDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; }

    public string? ParentId { get; set; } = null;

    public string Description { get; set; } = "";

    public int SortOrder { get; set; } = 99;

    public List<CategoryDto> SubCategories { get; init; } = [];
}