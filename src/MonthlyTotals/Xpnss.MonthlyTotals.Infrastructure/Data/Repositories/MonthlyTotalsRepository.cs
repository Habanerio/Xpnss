using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.MonthlyTotals.Domain.Entities;
using Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;
using Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Documents;
using Habanerio.Xpnss.MonthlyTotals.Infrastructure.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Repositories;

//TODO: Refactor. Create a private `GetAsync` method that takes nullables.
public class MonthlyTotalsRepository(
    IMongoDatabase mongoDb,
    ILogger<MonthlyTotalsRepository> logger,
    IMediator? mediator = null) :
    MongoDbRepository<MonthlyTotalDocument>(new MonthlyTotalsDbContext(mongoDb)),
    IMonthlyTotalsRepository
{
    private readonly ILogger<MonthlyTotalsRepository> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Adds or appends the MonthlyTotals for the user, entity, year, and month.
    /// </summary>
    /// <param name="monthlyTotal">The MonthlyTotal to be added or appended</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<MonthlyTotal?>> AddAsync(
        MonthlyTotal monthlyTotal,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(monthlyTotal.UserId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{monthlyTotal.UserId}`");

        ObjectId? entityObjectId = string.IsNullOrWhiteSpace(monthlyTotal.EntityId?.Value) ?
            ObjectId.Parse(monthlyTotal.EntityId?.Value) :
            null;

        var existingMonthlyTotalDoc = await FirstOrDefaultDocumentAsync(a =>
                a.UserId.Equals(monthlyTotal.UserId) &&
                (entityObjectId == null || a.EntityId.Equals(entityObjectId)) &&
                a.Year == monthlyTotal.Year &&
                a.Month == monthlyTotal.Month,
            cancellationToken);

        if (existingMonthlyTotalDoc is not null)
        {
            var updateResult = await UpdateAsync(monthlyTotal, cancellationToken);

            if (updateResult.IsFailed)
                return Result.Fail<MonthlyTotal?>("Could not update the MonthlyTotal");

            return updateResult!;
        }

        var monthlyTotalDoc = InfrastructureMapper.Map(monthlyTotal);

        if (monthlyTotalDoc is null)
            return Result.Fail<MonthlyTotal?>("Could not map the MonthlyTotal to its Document");

        await AddDocumentAsync(monthlyTotalDoc, cancellationToken);

        var newMonthlyTotal = InfrastructureMapper.Map(monthlyTotalDoc);

        return Result.Ok(newMonthlyTotal);
    }

    public async Task<Result<MonthlyTotal?>> GetAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        ObjectId? entityObjectId = string.IsNullOrWhiteSpace(entityId) ?
            null :
            ObjectId.Parse(entityId);

        //if (!ObjectId.TryParse(entityId, out var entityObjectId) ||
        //    entityObjectId.Equals(ObjectId.Empty))
        //    return Result.Fail($"Invalid EntityId: `{entityId}`");

        var monthlyTotalDoc = await FirstOrDefaultDocumentAsync(t =>
                t.UserId.Equals(userObjectId) &&
                //t.EntityId.Equals(entityObjectId) &&
                (entityObjectId == null || t.EntityId.Equals(entityObjectId)) &&
                (entityType == null || t.EntityType.Equals(entityType)) &&
                t.Year == year &&
                t.Month == month,
            cancellationToken);

        if (monthlyTotalDoc is null)
            return Result.Ok<MonthlyTotal?>(null);

        var monthlyTotal = InfrastructureMapper.Map(monthlyTotalDoc);

        if (monthlyTotal is null)
            return Result.Fail<MonthlyTotal?>("Could not map the MonthlyTotal Document to its Entity");

        return monthlyTotal;
    }

    public async Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        ObjectId? entityObjectId = string.IsNullOrWhiteSpace(entityId) ?
            null :
            ObjectId.Parse(entityId);

        //if (!ObjectId.TryParse(entityId, out var entityObjectId) ||
        //    entityObjectId.Equals(ObjectId.Empty))
        //    return Result.Fail($"Invalid EntityId: `{entityId}`");

        var monthlyTotalDocs = (await FindDocumentsAsync(t =>
                t.UserId.Equals(userObjectId) &&
                (entityObjectId == null || t.EntityId.Equals(entityObjectId)) &&
                (entityType == null || t.EntityType.Equals(entityType)),
            cancellationToken)).ToList();

        if (monthlyTotalDocs.Count == 0)
            return Result.Ok(Enumerable.Empty<MonthlyTotal>());

        var monthlyTotals = InfrastructureMapper.Map(monthlyTotalDocs);

        return Result.Ok(monthlyTotals);
    }

    public async Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        int year,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        ObjectId? entityObjectId = string.IsNullOrWhiteSpace(entityId) ?
            null :
            ObjectId.Parse(entityId);

        //if (!ObjectId.TryParse(entityId, out var entityObjectId) ||
        //    entityObjectId.Equals(ObjectId.Empty))
        //    return Result.Fail($"Invalid EntityId: `{entityId}`");

        var monthlyTotalDocs = (await FindDocumentsAsync(t =>
                t.UserId.Equals(userObjectId) &&
                (entityObjectId == null || t.EntityId.Equals(entityObjectId)) &&
                (entityType == null || t.EntityType.Equals(entityType)) &&
                t.Year.Equals(year),
            cancellationToken)).ToList();

        if (monthlyTotalDocs.Count == 0)
            return Result.Ok(Enumerable.Empty<MonthlyTotal>());

        var monthlyTotals = InfrastructureMapper.Map(monthlyTotalDocs);

        return Result.Ok(monthlyTotals);
    }

    public async Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        ObjectId? entityObjectId = string.IsNullOrWhiteSpace(entityId) ?
            null :
            ObjectId.Parse(entityId);

        //if (!ObjectId.TryParse(entityId, out var entityObjectId) ||
        //    entityObjectId.Equals(ObjectId.Empty))
        //    return Result.Fail($"Invalid EntityId: `{entityId}`");

        var monthlyTotalDocs = (await FindDocumentsAsync(t =>
                t.UserId.Equals(userObjectId) &&
                (entityObjectId == null || t.EntityId.Equals(entityObjectId)) &&
                (entityType == null || t.EntityType.Equals(entityType)) &&
                t.Year.Equals(year) &&
                t.Month.Equals(month),
            cancellationToken)).ToList();

        if (monthlyTotalDocs.Count == 0)
            return Result.Ok(Enumerable.Empty<MonthlyTotal>());

        var monthlyTotals = InfrastructureMapper.Map(monthlyTotalDocs);

        return Result.Ok(monthlyTotals);
    }

    public async Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityTypes.Keys? entityType,
        (int Year, int Month) startMonth,
        (int Year, int Month) endMonth,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        ObjectId? entityObjectId = string.IsNullOrWhiteSpace(entityId) ?
            null :
            ObjectId.Parse(entityId);

        //if (!ObjectId.TryParse(entityId, out var entityObjectId) ||
        //    entityObjectId.Equals(ObjectId.Empty))
        //    return Result.Fail($"Invalid EntityId: `{entityId}`");

        var monthlyTotalDocs = (await FindDocumentsAsync(t =>
                t.UserId.Equals(userObjectId) &&
                (entityObjectId == null || t.EntityId.Equals(entityObjectId)) &&
                (entityType == null || t.EntityType.Equals(entityType)) &&

                // Will this work? Could store a DateTime in the Document with yy/mm/01
                (
                    new DateOnly(t.Year, t.Month, 1) >= new DateOnly(startMonth.Year, startMonth.Month, 1) &&
                    new DateOnly(t.Year, t.Month, 1) <= new DateOnly(endMonth.Year, endMonth.Month, 1)
                ),
            cancellationToken)).ToList();

        if (monthlyTotalDocs.Count == 0)
            return Result.Ok(Enumerable.Empty<MonthlyTotal>());

        var monthlyTotals = InfrastructureMapper.Map(monthlyTotalDocs);

        return Result.Ok(monthlyTotals);
    }

    /// <summary>
    /// Updates an existing MonthlyTotal
    /// </summary>
    /// <returns></returns>
    private async Task<Result<MonthlyTotal>> UpdateAsync(
        MonthlyTotal monthlyTotal,
        CancellationToken cancellationToken = default)
    {
        var monthlyTotalDoc = InfrastructureMapper.Map(monthlyTotal);

        if (monthlyTotalDoc is null)
            return Result.Fail<MonthlyTotal>("Could not map the MonthlyTotal to its Document");

        try
        {
            var updateResult = await UpdateDocumentAsync(monthlyTotalDoc, cancellationToken);

            if (updateResult > 0)
                return Result.Ok(monthlyTotal);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not update the MonthlyTotal");

            return Result.Fail<MonthlyTotal>($"Could not update the MonthlyTotal: {e.Message}");
        }

        return Result.Fail<MonthlyTotal>("Could not update the MonthlyTotal");
    }
}