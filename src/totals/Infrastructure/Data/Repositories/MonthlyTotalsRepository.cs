using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Totals.Domain.Entities;
using Habanerio.Xpnss.Totals.Domain.Interfaces;
using Habanerio.Xpnss.Totals.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Totals.Infrastructure.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Totals.Infrastructure.Data.Repositories;

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
        Domain.Entities.MonthlyTotal monthlyTotal,
        CancellationToken cancellationToken = default)
    {
        ObjectId? objectId = !string.IsNullOrWhiteSpace(monthlyTotal.Id?.Value) ?
            ObjectId.Parse(monthlyTotal.Id?.Value) :
            ObjectId.GenerateNewId();

        if (!ObjectId.TryParse(monthlyTotal.UserId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{monthlyTotal.UserId}`");

        //ObjectId? entityObjectId = !string.IsNullOrWhiteSpace(monthlyTotal.EntityId?.Value) ?
        //    ObjectId.Parse(monthlyTotal.EntityId?.Value) :
        //    null;

        var existingMonthlyTotalDoc = await FirstOrDefaultDocumentAsync(a =>
                a.Id == objectId &&
                a.UserId.Equals(monthlyTotal.UserId) &&
                a.Year == monthlyTotal.Year &&
                a.Month == monthlyTotal.Month,
            cancellationToken);

        if (existingMonthlyTotalDoc is not null)
        {
            var updateResult = await UpdateAsync(monthlyTotal, cancellationToken);

            if (updateResult.IsFailed)
                return Result.Fail<Domain.Entities.MonthlyTotal?>("Could not update the MonthlyTotal");

            return updateResult!;
        }

        var monthlyTotalDoc = InfrastructureMapper.Map(monthlyTotal);

        if (monthlyTotalDoc is null)
            return Result.Fail<Domain.Entities.MonthlyTotal?>("Could not map the MonthlyTotal to its Document");

        await AddDocumentAsync(monthlyTotalDoc, cancellationToken);

        var newMonthlyTotal = InfrastructureMapper.Map(monthlyTotalDoc);

        return Result.Ok(newMonthlyTotal);
    }

    public async Task<Result<MonthlyTotal?>> GetAsync(
        string userId,
        string entityId,
        string subEntityId,
        EntityEnums.Keys entityType,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        ObjectId? entityObjectId = !string.IsNullOrWhiteSpace(entityId) ?
            ObjectId.Parse(entityId) :
            null;

        ObjectId? subEntityObjectId = !string.IsNullOrWhiteSpace(subEntityId) ?
            ObjectId.Parse(subEntityId) :
            null;

        //if (!ObjectId.TryParse(entityId, out var entityObjectId) ||
        //    entityObjectId.Equals(ObjectId.Empty))
        //    return Result.Fail($"Invalid EntityId: `{entityId}`");

        MonthlyTotal? monthlyTotal = null;

        try
        {
            var monthlyTotalDoc = await FirstOrDefaultDocumentAsync(t =>
                    t.UserId.Equals(userObjectId) &&
                    //t.EntityId.Equals(entityObjectId) &&
                    (entityObjectId == null || t.EntityId.Equals(entityObjectId)) &&
                    (subEntityObjectId == null || t.SubEntityId.Equals(subEntityObjectId)) &&
                    (entityType == null || t.EntityType.Equals(entityType)) &&
                    t.Year == year &&
                    t.Month == month,
                cancellationToken);

            if (monthlyTotalDoc is null)
                return Result.Ok<MonthlyTotal?>(null);

            monthlyTotal = InfrastructureMapper.Map(monthlyTotalDoc);

            if (monthlyTotal is null)
                return Result.Fail<MonthlyTotal?>("Could not map the MonthlyTotal Document to its Entity");

            return monthlyTotal;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public async Task<Result<IEnumerable<MonthlyTotal>>> ListAsync(
        string userId,
        string entityId,
        EntityEnums.Keys entityType,
        int year,
        CancellationToken cancellationToken = default)
    {
        var results = await RangeAsync(userId, entityId, entityType, (year, 1), (year, 12), cancellationToken);

        return results;

        //if (!ObjectId.TryParse(userId, out var userObjectId) ||
        //    userObjectId.Equals(ObjectId.Empty))
        //    return Result.Fail($"Invalid UserId: `{userId}`");

        //ObjectId? entityObjectId = string.IsNullOrWhiteSpace(entityId) ?
        //    null :
        //    ObjectId.Parse(entityId);

        //var monthlyTotalDocs = (await FindDocumentsAsync(t =>
        //        t.UserId.Equals(userObjectId) &&
        //        (entityObjectId == null || t.EntityId.Equals(entityObjectId)) &&
        //        (entityType == null || t.EntityType.Equals(entityType)) &&
        //        t.Year.Equals(year),
        //    cancellationToken)).ToList();

        //if (monthlyTotalDocs.Count == 0)
        //    return Result.Ok(Enumerable.Empty<MonthlyTotal>());

        //var monthlyTotals = InfrastructureMapper.Map(monthlyTotalDocs);

        //if (monthlyTotals is null)
        //    throw new InvalidCastException($"{GetType()}: Failed to map MonthlyTotalDocument to MonthlyTotal");

        //return Result.Ok(monthlyTotals);
    }

    public async Task<Result<IEnumerable<MonthlyTotal>>> RangeAsync(
        string userId,
        string entityId,
        EntityEnums.Keys entityType,
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
        Domain.Entities.MonthlyTotal monthlyTotal,
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