using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.PayerPayees.Domain;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Documents;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Mappers;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Repositories;

/// <summary>
/// Responsible for managing the persistence of Merchant Documents.
/// </summary>
/// <param name="options">Db settings to connect to the Mongo Db</param>
/// <param name="mediator"></param>
public class PayerPayeesRepository(
    IOptions<MongoDbSettings> options,
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
            return Result.Fail("Failed to map Merchant to MerchantDocument");

        await AddDocumentAsync(payerPayeeDoc, cancellationToken);

        //HandleDomainEvents(payerPayee);

        return payerPayee;
    }

    public async Task<Result<PayerPayee?>> GetAsync(
        string userId,
        string payerPayeeId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(payerPayeeId, out var payerPayeeObjectId) ||
            payerPayeeObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid PayerPayeeId: `{payerPayeeId}`");

        var payerPayeeDoc = await FirstOrDefaultDocumentAsync(a =>
                a.Id.Equals(payerPayeeObjectId) && a.UserId.Equals(userId),
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
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var payerPayeeIdList = payerPayeeIds?.ToList() ?? [];

        if (!payerPayeeIdList.Any())
            return Result.Fail("Ids cannot be null or empty");

        var docs = await FindDocumentsAsync(a =>
            a.UserId == userId && payerPayeeIdList.Contains(a.Id), cancellationToken);

        var payerPayees = InfrastructureMapper.Map(docs);

        return Result.Ok(payerPayees);
    }

    public async Task<Result<IEnumerable<PayerPayee>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var docs = await FindDocumentsAsync(a =>
            a.UserId == userId,
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