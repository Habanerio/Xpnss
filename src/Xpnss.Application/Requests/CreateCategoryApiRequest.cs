using System.Text.Json.Serialization;
using Habanerio.Xpnss.Domain.Types;

namespace Habanerio.Xpnss.Application.Requests;

public sealed record CreateCategoryApiRequest : UserRequiredApiRequest
{
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("CategoryType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CategoryGroupEnums.CategoryKeys CategoryType { get; set; } = CategoryGroupEnums.CategoryKeys.EXPENSE;

    public string Description { get; set; } = string.Empty;

    [JsonConstructor]
    public CreateCategoryApiRequest() { }

    public CreateCategoryApiRequest(
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