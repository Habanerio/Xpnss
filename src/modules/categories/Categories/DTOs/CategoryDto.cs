namespace Habanerio.Xpnss.Modules.Categories.DTOs;

public sealed record CategoryDto
{
    public string Id { get; init; }

    public string UserId { get; init; }

    public string Name { get; init; }

    public string? ParentId { get; init; } = null;

    public string Description { get; init; } = "";

    public int SortOrder { get; init; } = 99;

    public IReadOnlyCollection<CategoryDto> SubCategories { get; init; } = [];
}