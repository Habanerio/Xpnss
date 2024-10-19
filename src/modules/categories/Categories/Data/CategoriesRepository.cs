using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Categories.Data;

public class CategoriesRepository : MongoDbRepository<CategoryDocument>, ICategoriesRepository
{
    protected CategoriesRepository(IOptions<MongoDbSettings> options) : base(new CategoriesDbContext(options))
    { }


    public async Task<Result<CategoryDocument>> AddAsync(CategoryDocument category, CancellationToken cancellationToken = default)
    {
        category.DateCreated = DateTime.UtcNow;

        await base.AddDocumentAsync(category, cancellationToken);

        return Result.Ok(category);
    }

    public async Task<Result<CategoryDocument>> GetByIdAsync(
        string userId,
        string parentCategoryId,
        string childCategoryId = "",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(parentCategoryId, out var parentObjectId) ||
            parentObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid Parent CategoryId: `{parentCategoryId}`");

        var childObjectId = ObjectId.Empty;

        if (!string.IsNullOrWhiteSpace(childCategoryId))
        {
            if (!ObjectId.TryParse(childCategoryId, out childObjectId))
                return Result.Fail($"Invalid Child CategoryId: `{childCategoryId}`");
        }

        var parentDoc = await FirstOrDefaultAsync(a =>
                                    a.Id.Equals(parentObjectId) && a.UserId.Equals(userId),
                                cancellationToken);

        if (parentDoc is null)
            return Result.Fail($"Parent Category not found for Parent CategoryId: `{parentCategoryId}`");

        if (childObjectId.Equals(ObjectId.Empty))
            return Result.Ok(parentDoc);

        var childDoc = parentDoc.SubCategories.Find(c => c.Id.Equals(childObjectId));

        if (childDoc is null)
            return Result.Fail($"Child Category not found for Child CategoryId: `{childCategoryId}`");

        return Result.Ok(childDoc);
    }

    public async Task<Result<IEnumerable<CategoryDocument>>> ListAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var docs = (await FindAsync(a =>
            a.UserId == userId, cancellationToken));

        return Result.Ok(docs);
    }

    public async Task<Result<CategoryDocument>> UpdateAsync(string userId, CategoryDocument category, CancellationToken cancellationToken = default)
    {
        category.DateUpdated = DateTime.UtcNow;

        var saveCount = await UpdateDocumentAsync(category, cancellationToken);

        if (saveCount == 0)
            return Result.Fail("Could not update the Category");

        return Result.Ok();
    }
}