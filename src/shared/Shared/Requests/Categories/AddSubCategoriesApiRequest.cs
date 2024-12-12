using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Habanerio.Xpnss.Shared.Requests;

namespace Habanerio.Xpnss.Shared.Requests.Categories;

public record AddSubCategoriesApiRequest : UserRequiredRequest
{
    [Required]
    public string ParentCategoryId { get; set; } = string.Empty;

    [Required]
    public IEnumerable<AddSubCategoriesRequestItem> SubCategories { get; set; } = [];

    [JsonConstructor]
    public AddSubCategoriesApiRequest() { }

    public AddSubCategoriesApiRequest(
        string userId,
        string parentCategoryId,
        IEnumerable<AddSubCategoriesRequestItem> subCategories)
    {
        UserId = userId;
        ParentCategoryId = parentCategoryId;
        SubCategories = subCategories;
    }
}

public record AddSubCategoriesRequestItem
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int SortOrder { get; set; } = 999;
}