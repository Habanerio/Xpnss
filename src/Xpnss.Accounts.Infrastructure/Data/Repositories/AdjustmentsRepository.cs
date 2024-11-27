using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Accounts.Domain.Entities;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Accounts.Infrastructure.Mappers;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data.Repositories;

public class AdjustmentsRepository(
    IOptions<MongoDbSettings> options,
    IMongoDatabase mongoDb,
    IMediator? mediator = null) :
    MongoDbRepository<AdjustmentDocument>(new AccountsDbContext(mongoDb)),
    IAdjustmentsRepository
{
    public async Task<Result<Adjustment>> AddAsync(
        Adjustment adjustment,
        CancellationToken cancellationToken = default)
    {
        if (adjustment is null)
            return Result.Fail("Adjustment cannot be null");

        try
        {
            var adjustmentDoc = InfrastructureMapper.Map(adjustment);

            if (adjustmentDoc is null)
                return Result.Fail("Could not map the Adjustment to its Document");

            await AddDocumentAsync(adjustmentDoc, cancellationToken);

            return adjustment;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not save the Adjustment{Environment.NewLine}{e.Message}");
        }
    }

    public async Task<Result<Adjustment?>> GetAsync(
        string userId,
        string adjustmentId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(adjustmentId, out var adjustmentObjectId))
            return Result.Fail("AdjustmentId is not a valid ObjectId");

        var adjustmentDoc = await FirstOrDefaultDocumentAsync(a =>
                a.Id.Equals(adjustmentObjectId) && a.UserId.Equals(userId)
            , cancellationToken);

        if (adjustmentDoc is null)
            return Result.Ok<Adjustment?>(null);

        var adjustment = InfrastructureMapper.Map(adjustmentDoc);

        if (adjustment is null)
            return Result.Fail<Adjustment?>("Could not map the Adjustment Document to its Entity");

        return Result.Ok<Adjustment?>(adjustment);
    }

    public async Task<Result<IEnumerable<Adjustment>>> ListAsync(
        string userId,
        string accountId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
            accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{accountId}`");

        var adjustmentDocs = await FindDocumentsAsync(a =>
            a.UserId.Equals(userId) && a.AccountId.Equals(accountObjectId), cancellationToken);

        var adjustments = InfrastructureMapper.Map(adjustmentDocs);

        return Result.Ok(adjustments);
    }

    public async Task<Result<Adjustment>> UpdateAsync(
        Adjustment adjustment,
        CancellationToken cancellationToken = default)
    {
        if (adjustment is null)
            return Result.Fail("Adjustment cannot be null");

        try
        {
            var adjustmentDoc = InfrastructureMapper.Map(adjustment);

            if (adjustmentDoc is null)
                return Result.Fail("Could not map the Adjustment to its Document");

            await UpdateDocumentAsync(adjustmentDoc, cancellationToken);

            return adjustment;
        }
        catch (Exception e)
        {
            return Result.Fail($"Could not update the Adjustment{Environment.NewLine}{e.Message}");
        }
    }

    public async Task<Result> DeleteAsync(
        string userId,
        string adjustmentId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(adjustmentId, out var adjustmentObjectId) ||
            adjustmentObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AdjustmentId: `{adjustmentId}`");

        var adjustmentDoc = await FirstOrDefaultDocumentAsync(a =>
            a.Id.Equals(adjustmentObjectId) &&
            a.UserId.Equals(userId),
            cancellationToken);

        if (adjustmentDoc is null)
            return Result.Fail("Adjustment not found");

        var isDeleted = await RemoveDocumentAsync(adjustmentDoc, cancellationToken);

        return isDeleted ? Result.Ok() : Result.Fail("Could not delete Adjustment");
    }
}