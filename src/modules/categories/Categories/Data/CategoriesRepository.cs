using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Categories.Data;

public class CategoriesRepository(IOptions<MongoDbSettings> options)
    : MongoDbRepository<CategoryDocument>(new CategoriesDbContext(options)), ICategoriesRepository
{
    public async Task<Result<CategoryDocument>> AddAsync(CategoryDocument category, CancellationToken cancellationToken = default)
    {
        category.DateCreated = DateTime.UtcNow;

        await base.AddDocumentAsync(category, cancellationToken);

        return Result.Ok(category);
    }

    public async Task<Result<CategoryDocument>> GetByIdAsync(
        string userId,
        string categoryId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(categoryId, out var parentObjectId) ||
            parentObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid Parent CategoryId: `{categoryId}`");

        var parentDoc = await FirstOrDefaultAsync(a =>
                a.Id.Equals(parentObjectId) && a.UserId.Equals(userId),
            cancellationToken);

        if (parentDoc is null)
            return Result.Fail($"Parent Category not found for Parent CategoryId: `{categoryId}`");

        return Result.Ok(parentDoc);
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