using Habanerio.Xpnss.Modules.Categories.Data;
using Habanerio.Xpnss.Modules.Categories.DTOs;

namespace Habanerio.Xpnss.Modules.Categories.Mappers;

public class DocumentToDtoMappings
{
    public static IEnumerable<CategoryDto> Map(IEnumerable<CategoryDocument> documents)
    {
        var results = new List<CategoryDto>();

        foreach (var document in documents)
        {
            var dto = Map(document);

            if (dto is not null)
                results.Add(dto);
        }

        return results;
    }

    public static CategoryDto? Map(CategoryDocument? document)
    {
        if (document is null)
            return default;

        var dto = new CategoryDto
        {
            Id = document.Id.ToString(),
            UserId = document.UserId,
            Name = document.Name,
            ParentId = document.ParentId.ToString(),
            Description = document.Description,
            SortOrder = document.SortOrder,
            SubCategories = Map(document.SubCategories)?.ToList().AsReadOnly() ?? new List<CategoryDto>().AsReadOnly()
        };

        return dto;
    }
}