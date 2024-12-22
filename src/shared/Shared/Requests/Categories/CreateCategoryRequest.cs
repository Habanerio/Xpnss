using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Shared.Requests.Categories;

public sealed record CreateCategoryRequest : UserRequiredRequest
{
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("CategoryType")]
    [JsonConverter(typeof(JsonNumberEnumConverter<CategoryGroupEnums.CategoryKeys>))]
    public CategoryGroupEnums.CategoryKeys CategoryType { get; set; } = CategoryGroupEnums.CategoryKeys.EXPENSES;

    public string Description { get; set; } = string.Empty;

    [JsonConstructor]
    public CreateCategoryRequest() { }

    public CreateCategoryRequest(
        string userId,
        string name,
        CategoryGroupEnums.CategoryKeys categoryType,
        string description)
    {
        UserId = userId;
        CategoryType = categoryType;
        Name = name;
        Description = description;
    }
}