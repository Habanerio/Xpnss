using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.PayerPayees.Domain.Entities;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Documents;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Mappers;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Repositories;

/// <summary>
/// Responsible for managing the persistence of Merchant Documents.
/// </summary>
/// <param name="mediator"></param>
public class PayerPayeesRepository(
    IMongoDatabase mongoDb,
    IMediator? mediator = null)
    : MongoDbRepository<PayerPayeeDocument>(new PayerPayeesDbContext(mongoDb)), IPayerPayeesRepository
{
    private readonly IMediator? _mediator = mediator;

    public async Task<Result<PayerPayee>> AddAsync(
        PayerPayee payerPayee,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(payerPayee.Name))
            return Result.Fail($"{nameof(payerPayee.Name)} cannot be null or whitespace");

        var payerPayeeDoc = InfrastructureMapper.Map(payerPayee);

        if (payerPayeeDoc is null)
            return Result.Fail("Failed to map PayerPayee to PayerPayeeDocument");

        await AddDocumentAsync(payerPayeeDoc, cancellationToken);

        var newPayerPayee = InfrastructureMapper.Map(payerPayeeDoc);

        if (newPayerPayee is null)
            return Result.Fail("Failed to map PayerPayeeDocument to PayerPayee");

        //HandleDomainEvents(payerPayee);

        return newPayerPayee;
    }

    public async Task<Result<PayerPayee?>> GetAsync(
        string userId,
        string payerPayeeId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        if (!ObjectId.TryParse(payerPayeeId, out var payerPayeeObjectId) ||
            payerPayeeObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid PayerPayeeId: `{payerPayeeId}`");

        var payerPayeeDoc = await FirstOrDefaultDocumentAsync(a =>
                a.Id.Equals(payerPayeeObjectId) &&
                a.UserId.Equals(userObjectId),
            cancellationToken);

        if (payerPayeeDoc is null)
            return Result.Ok<PayerPayee?>(default);

        var payerPayee = InfrastructureMapper.Map(payerPayeeDoc);

        if (payerPayee is null)
            return Result.Fail("Failed to map MerchantDocument to Merchant");

        return Result.Ok<PayerPayee?>(payerPayee);
    }

    public async Task<Result<IEnumerable<PayerPayee>>> GetAsync(
        string userId,
        IEnumerable<ObjectId> payerPayeeIds,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        var payerPayeeIdList = payerPayeeIds?.ToList() ?? [];

        if (!payerPayeeIdList.Any())
            return Result.Fail("Ids cannot be null or empty");

        var docs = await FindDocumentsAsync(a =>
            a.UserId.Equals(userObjectId) &&
            payerPayeeIdList.Contains(a.Id), cancellationToken);

        var payerPayees = InfrastructureMapper.Map(docs);

        return Result.Ok(payerPayees);
    }

    public async Task<Result<IEnumerable<PayerPayee>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid UserId: `{userId}`");

        var docs = await FindDocumentsAsync(a =>
            a.UserId.Equals(userObjectId),
            cancellationToken);

        var payerPayees = InfrastructureMapper.Map(docs);

        return Result.Ok(payerPayees);
    }

    //private void HandleIntegrationEvents(Merchant entity)
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