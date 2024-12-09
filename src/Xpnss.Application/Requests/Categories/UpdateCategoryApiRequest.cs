using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Application.Requests.Categories;

public sealed record UpdateCategoryApiRequest : UserRequiredApiRequest
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public int SortOrder { get; set; }

    [JsonConstructor]
    public UpdateCategoryApiRequest()
    { }

    public UpdateCategoryApiRequest(
        string userId,
        string id,
        string name,
        string description,
        int sortOrder)
    {
        UserId = userId;
        Id = id;
        Name = name;
        Description = description;
        SortOrder = sortOrder;
    }
}

public sealed record UpdateSubCategoryApiRequest : UserRequiredApiRequest
{
    public string Id { get; set; } = "";

    public string ParentId { get; set; } = "";

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public int SortOrder { get; set; }
}