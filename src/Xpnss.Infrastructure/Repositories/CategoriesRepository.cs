using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Domain.Categories;
using Habanerio.Xpnss.Domain.Categories.Interfaces;
using Habanerio.Xpnss.Infrastructure.Documents;
using Habanerio.Xpnss.Infrastructure.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Infrastructure.Repositories;

public class CategoriesRepository(IOptions<MongoDbSettings> options)
    : MongoDbRepository<CategoryDocument>(new XpnssDbContext(options)), ICategoriesRepository
{
    public async Task<Result<Category>> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (category is null)
            return Result.Fail("Category cannot be null");

        var categoryDoc = Mapper.Map(category);

        if (categoryDoc is null)
            return Result.Fail("Could not map the Category to CategoryDocument");

        categoryDoc.DateCreated = DateTime.UtcNow;

        await AddDocumentAsync(categoryDoc, cancellationToken);

        var newCategory = Mapper.Map(categoryDoc);

        return Result.Ok(newCategory!);
    }

    public async Task<Result<Category?>> GetAsync(
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

        // Could not be found
        if (parentDoc is null)
            return Result.Ok<Category?>(null);

        var parentCategory = Mapper.Map(parentDoc);

        if (parentCategory is null)
            return Result.Fail("Could not map the CategoryDocument to Category");

        return Result.Ok<Category?>(parentCategory);
    }

    public async Task<Result<IEnumerable<Category>>> ListAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var docs = (await FindAsync(a =>
            a.UserId == userId, cancellationToken))
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name);

        var categories = Mapper.Map(docs);

        return Result.Ok(categories);
    }

    public async Task<Result<Category>> UpdateAsync(string userId, Category category, CancellationToken cancellationToken = default)
    {
        var categoryDoc = Mapper.Map(category);

        if (categoryDoc is null)
            return Result.Fail("Could not map the Category to CategoryDocument");

        categoryDoc.DateUpdated = DateTime.UtcNow;

        var saveCount = await UpdateDocumentAsync(categoryDoc, cancellationToken);

        if (saveCount == 0)
            return Result.Fail("Could not update the Category");

        return Result.Ok();
    }
}