using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Categories.Domain.Entities;

namespace Habanerio.Xpnss.Categories.Application.Mappers;

internal static partial class ApplicationMapper
{
    public static IEnumerable<CategoryDto> Map(IEnumerable<Category> documents)
    {
        return documents
            .Select(Map)
            .Where(d => d is not null)
            .Cast<CategoryDto>();
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
            Description = document.Description,
            SortOrder = document.SortOrder,
            SubCategories = Map(document.SubCategories)?.ToList() ?? []
        };

        return dto;
    }

    public static SubCategoryDto? Map(SubCategory? document)
    {
        if (document is null)
            return default;

        var dto = new SubCategoryDto
        {
            Id = document.Id,
            Name = document.Name,
            Description = document.Description,
            ParentId = document.ParentId,
            SortOrder = document.SortOrder
        };

        return dto;
    }

    public static IEnumerable<SubCategoryDto> Map(IEnumerable<SubCategory> documents)
    {
        return documents.Select(Map)
            .Where(d => d is not null)
            .Cast<SubCategoryDto>();
    }
}