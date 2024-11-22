using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Merchants.Domain;
using Habanerio.Xpnss.Merchants.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Merchants.Infrastructure;

/// <summary>
/// Responsible for managing the persistence of Merchant Documents.
/// </summary>
/// <param name="options">Db settings to connect to the Mongo Db</param>
/// <param name="mediator"></param>
public class MerchantsRepository(
    IOptions<MongoDbSettings> options,
    IMediator? mediator = null)
    : MongoDbRepository<MerchantDocument>(new MerchantsDbContext(options)), IMerchantsRepository
{
    private readonly IMediator? _mediator = mediator;

    public async Task<Result<Merchant>> AddAsync(
        Merchant merchant,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(merchant.Name))
            return Result.Fail($"{nameof(merchant.Name)} cannot be null or whitespace");

        var merchantDoc = Mapper.Map(merchant);

        if (merchantDoc is null)
            return Result.Fail("Failed to map Merchant to MerchantDocument");

        await AddDocumentAsync(merchantDoc, cancellationToken);

        //HandleDomainEvents(merchant);

        return merchant;
    }

    public async Task<Result<Merchant?>> GetAsync(
        string userId,
        string merchantId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(merchantId, out var merchantObjectId) ||
            merchantObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid MerchantId: `{merchantId}`");

        var merchantDoc = await FirstOrDefaultDocumentAsync(a =>
                a.Id.Equals(merchantObjectId) && a.UserId.Equals(userId),
            cancellationToken);

        if (merchantDoc is null)
            return Result.Ok<Merchant>(null);

        var merchant = Mapper.Map(merchantDoc);

        if (merchant is null)
            return Result.Fail("Failed to map MerchantDocument to Merchant");

        return merchant;
    }

    public async Task<Result<IEnumerable<Merchant>>> GetAsync(
        string userId,
        IEnumerable<ObjectId> merchantIds,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var merchantIdList = merchantIds?.ToList() ?? [];

        if (!merchantIdList.Any())
            return Result.Fail("Ids cannot be null or empty");

        var docs = await FindDocumentsAsync(a => a.UserId == userId && merchantIdList.Contains(a.Id), cancellationToken);

        var merchants = Mapper.Map(docs);

        return Result.Ok(merchants);
    }

    public async Task<Result<IEnumerable<Merchant>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var docs = await FindDocumentsAsync(a =>
            a.UserId == userId, cancellationToken);

        var merchants = Mapper.Map(docs);

        return Result.Ok(merchants);
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