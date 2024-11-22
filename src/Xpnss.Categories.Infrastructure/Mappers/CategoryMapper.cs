using Habanerio.Xpnss.Categories.Domain;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Domain.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Categories.Infrastructure.Mappers;

public static partial class Mapper
{
    public static Category? Map(CategoryDocument? document)
    {
        if (document is null)
            return null;

        return Category.Load(
            new CategoryId(document.Id),
            new UserId(document.UserId),
            new CategoryName(document.Name),
            document.Description,
            document.SortOrder,
            new CategoryId(document.ParentId),
            Map(document.SubCategories),
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);

    }

    public static IEnumerable<Category> Map(IEnumerable<CategoryDocument> documents)
    {
        return documents.Select(Map)
            .Where(x => x is not null)
            .Cast<Category>();
    }

    public static CategoryDocument? Map(Category? entity)
    {
        if (entity is null)
            return null;

        return new CategoryDocument()
        {
            Id = ObjectId.Parse(entity.Id),
            UserId = entity.UserId,
            Name = entity.Name,
            Description = entity.Description,
            SortOrder = entity.SortOrder,
            SubCategories = Map(entity.SubCategories).ToList(),
            ParentId = string.IsNullOrWhiteSpace(entity.ParentId) ?
                null :
                ObjectId.Parse(entity.ParentId),
            DateCreated = entity.DateCreated,
            DateUpdated = entity.DateUpdated,
            DateDeleted = entity.DateDeleted
        };
    }

    public static IEnumerable<CategoryDocument> Map(IEnumerable<Category> entities)
    {
        return entities.Select(Map)
            .Where(x => x is not null)
            .Cast<CategoryDocument>();
    }
}