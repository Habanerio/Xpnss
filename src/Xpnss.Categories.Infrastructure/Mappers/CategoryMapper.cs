using Habanerio.Xpnss.Categories.Domain.Entities;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Categories.Infrastructure.Mappers;

public static partial class InfrastructureMapper
{
    /// <summary>
    /// Maps a Category Document to a Category Entity
    /// </summary>
    /// <returns></returns>
    public static Category? Map(CategoryDocument? document)
    {
        if (document is null)
            return null;

        return Category.Load(
            new CategoryId(document.Id),
            new UserId(document.UserId),
            new CategoryName(document.Name),
            document.CategoryType,
            document.Description,
            document.SortOrder,
            Map(document.SubCategories),
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);

    }

    /// <summary>
    /// Maps a collection of Category Documents to a collection of Category Entities
    /// </summary>
    /// <param name="documents"></param>
    /// <returns></returns>
    public static IEnumerable<Category> Map(IEnumerable<CategoryDocument> documents)
    {
        return documents.Select(Map)
            .Where(x => x is not null)
            .Cast<Category>();
    }

    /// <summary>
    /// Maps a Category Entity to a Category Document
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static CategoryDocument? Map(Category? entity)
    {
        if (entity is null)
            return null;

        return new CategoryDocument()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            CategoryType = entity.CategoryType,
            Description = entity.Description,
            IsDeleted = entity.IsDeleted,
            SortOrder = entity.SortOrder,
            SubCategories = Map(entity.SubCategories).ToList(),
            DateCreated = entity.DateCreated,
            DateUpdated = entity.DateUpdated,
            DateDeleted = entity.DateDeleted
        };
    }

    /// <summary>
    /// Maps a collection of Category Entities to a collection of Category Documents
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<CategoryDocument> Map(IEnumerable<Category> entities)
    {
        return entities.Select(Map)
            .Where(x => x is not null)
            .Cast<CategoryDocument>();
    }

    /// <summary>
    /// Maps a SubCategory Document to a SubCategory Entity
    /// </summary>
    /// <returns></returns>
    public static SubCategoryDocument? Map(SubCategory? entity)
    {
        if (entity is null)
            return null;

        return new SubCategoryDocument
        {
            Id = entity.Id,
            ParentId = entity.ParentId,
            Name = entity.Name,
            Description = entity.Description,
            IsDeleted = entity.IsDeleted,
            SortOrder = entity.SortOrder,
            DateCreated = entity.DateCreated,
            DateUpdated = entity.DateUpdated,
            DateDeleted = entity.DateDeleted
        };
    }

    /// <summary>
    /// Maps a collection of SubCategory Documents to a collection of SubCategory Entities
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SubCategoryDocument> Map(IEnumerable<SubCategory> entities)
    {
        return entities.Select(Map)
            .Where(x => x is not null)
            .Cast<SubCategoryDocument>();
    }

    /// <summary>
    /// Maps a SubCategory Entity to a SubCategory Document
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public static SubCategory? Map(SubCategoryDocument? document)
    {
        if (document is null)
            return null;

        return SubCategory.Load(
            new SubCategoryId(document.Id),
            new CategoryId(document.ParentId),
            new CategoryName(document.Name),
            document.CategoryType,
            document.Description,
            document.SortOrder,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
    }

    /// <summary>
    /// Maps a collection of SubCategory Documents to a collection of SubCategory Entities
    /// </summary>
    /// <param name="documents"></param>
    /// <returns></returns>
    public static IEnumerable<SubCategory> Map(IEnumerable<SubCategoryDocument> documents)
    {
        var subCategories = documents.Select(Map)
            .Where(x => x is not null)
            .Cast<SubCategory>();

        return subCategories
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name);
    }
}