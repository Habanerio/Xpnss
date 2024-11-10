using Habanerio.Xpnss.Domain.Categories;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.Documents;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Infrastructure.Mappers;

/// <summary>
/// Responsible for mapping Category Domain Entities to Category Documents and vice versa
/// </summary>
internal static partial class Mapper
{
    /// <summary>
    /// Maps from a Category Document to a Category Domain Entity
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public static Category? Map(CategoryDocument? document)
    {
        if (document == null)
            return null;

        var category = Category.Load(
            new CategoryId(document.Id.ToString()),
            new UserId(document.UserId),
            new CategoryName(document.Name),
            document.Description,
            document.SortOrder,
            document.ParentId is null ? CategoryId.New : new CategoryId(document.ParentId.ToString()!),
            Map(document.SubCategories),
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);

        return category;
    }

    /// <summary>
    /// Maps a collection of Category Documents to a collection of Category Domain Entities
    /// </summary>
    /// <param name="documents"></param>
    /// <returns></returns>
    public static IEnumerable<Category> Map(IEnumerable<CategoryDocument> documents)
    {
        return documents.Select(Map).Where(x => x is not null).Cast<Category>();
    }

    /// <summary>
    /// Maps from a Category Domain Entity to a Category Document
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static CategoryDocument? Map(Category? category)
    {
        if (category is null)
            return null;

        var document = new CategoryDocument(
            ObjectId.Parse(category.Id),
            category.UserId,
            category.Name,
            category.Description,
            category.SortOrder,
            Map(category.SubCategories),
            string.IsNullOrWhiteSpace(category.ParentId) ? null : ObjectId.Parse(category.ParentId));

        return document;
    }

    /// <summary>
    /// Maps a collection of Category Domain Entities to a collection of Category Documents
    /// </summary>
    /// <param name="categories"></param>
    /// <returns></returns>
    public static List<CategoryDocument> Map(IReadOnlyCollection<Category> categories)
    {
        return categories.Select(Map).Where(x => x is not null).Cast<CategoryDocument>().ToList();
    }
}