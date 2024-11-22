using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Domain;

namespace Habanerio.Xpnss.Categories.Application.Mappers;


internal static partial class Mapper
{
    public static IEnumerable<CategoryDto> Map(IEnumerable<Category> documents)
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

    public static CategoryDto? Map(Category? document)
    {
        if (document is null)
            return default;

        var dto = new CategoryDto
        {
            Id = document.Id,
            UserId = document.UserId,
            Name = document.Name,
            ParentId = document.ParentId,
            Description = document.Description,
            SortOrder = document.SortOrder,
            SubCategories = Map(document.SubCategories)?.ToList().AsReadOnly() ?? new List<CategoryDto>().AsReadOnly()
        };

        return dto;
    }
}