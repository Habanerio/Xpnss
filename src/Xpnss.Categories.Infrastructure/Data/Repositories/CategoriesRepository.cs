using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Categories.Domain.Entities;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Categories.Infrastructure.Mappers;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Categories.Infrastructure.Data.Repositories;

/// <summary>
/// Responsible for managing the persistence of Category Documents.
/// </summary>
/// <param name="mediator"></param>
public class CategoriesRepository(
    IMongoDatabase mongoDb,
    IMediator? mediator = null)
    : MongoDbRepository<CategoryDocument>(new CategoriesDbContext(mongoDb)), ICategoriesRepository
{
    private readonly IMediator? _mediator = mediator;

    public async Task<Result<Category>> AddAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        if (category is null)
            return Result.Fail("Category cannot be null");

        var categoryDoc = InfrastructureMapper.Map(category);

        if (categoryDoc is null)
            return Result.Fail("Could not map the Category to CategoryDocument");

        await AddDocumentAsync(categoryDoc, cancellationToken);

        // Need to map back to entity to get the Id
        var newCategory = InfrastructureMapper.Map(categoryDoc);

        if (newCategory is null)
            return Result.Fail("Could not map the CategoryDocument to Category");

        //HandleDomainEvents(category);

        return Result.Ok(newCategory);
    }

    public async Task<Result<bool>> ExistsAsync(
        string userId,
        string name,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail("Name cannot be null or empty");

        var rslt = await FirstOrDefaultDocumentAsync(c =>
                c.UserId.Equals(userObjectId) &&
                c.Name.Equals(name),
            cancellationToken);

        var doesExist = rslt is not null;

        return Result.Ok(doesExist);
    }

    public async Task<Result<Category?>> GetAsync(
        string userId,
        string categoryId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        if (!ObjectId.TryParse(categoryId, out var parentObjectId) ||
            parentObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid Parent CategoryId: `{categoryId}`");

        var parentDoc = await FirstOrDefaultDocumentAsync(a =>
                a.UserId.Equals(userObjectId) &&
                a.Id.Equals(parentObjectId),
            cancellationToken);

        // Could not be found
        if (parentDoc is null)
            return Result.Ok<Category?>(null);

        var parentCategory = InfrastructureMapper.Map(parentDoc);

        if (parentCategory is null)
            return Result.Fail("Could not map the CategoryDocument to Category");

        return Result.Ok<Category?>(parentCategory);
    }

    public async Task<Result<IEnumerable<Category>>> ListAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        var docs = (await FindDocumentsAsync(a =>
            a.UserId == userObjectId, cancellationToken))
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name);

        var categories = InfrastructureMapper.Map(docs);

        return Result.Ok(categories);
    }

    public async Task<Result<Category>> UpdateAsync(string userId, Category category, CancellationToken cancellationToken = default)
    {
        var categoryDoc = InfrastructureMapper.Map(category);

        if (categoryDoc is null)
            return Result.Fail("Could not map the Category to CategoryDocument");

        var saveCount = await UpdateDocumentAsync(categoryDoc, cancellationToken);

        if (saveCount == 0)
            return Result.Fail("Could not update the Category");

        //HandleDomainEvents(category);

        return Result.Ok(category);
    }

    //private void HandleIntegrationEvents(Category entity)
    //{
    //    if (_mediator is null)
    //        return;

    //    foreach (var @event in entity.DomainEvents)
    //    {
    //        //await _eventDispatcher.DispatchAsync(@event);
    //        _mediator.Send(@event);
    //    }

    //    entity.ClearDomainEvents();
    //}
}