using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.DTOs;

public sealed record CategoryDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; }

    [JsonPropertyName("CategoryType")]
    [JsonConverter(typeof(JsonNumberEnumConverter<CategoryGroupEnums.CategoryKeys>))]
    public CategoryGroupEnums.CategoryKeys CategoryType { get; set; } = CategoryGroupEnums.CategoryKeys.EXPENSE;

    public string CategoryTypeString => CategoryType.ToString();

    public string Description { get; set; } = string.Empty;

    public int SortOrder { get; set; } = 999;

    public List<SubCategoryDto> SubCategories { get; init; } = [];
}

public sealed record SubCategoryDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ParentId { get; set; }

    public int SortOrder { get; set; } = 999;
}