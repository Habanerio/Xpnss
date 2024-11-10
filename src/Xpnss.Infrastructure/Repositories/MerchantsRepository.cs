using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Domain.Merchants;
using Habanerio.Xpnss.Domain.Merchants.Interfaces;
using Habanerio.Xpnss.Infrastructure.Documents;
using Habanerio.Xpnss.Infrastructure.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Infrastructure.Repositories;

public class MerchantsRepository(IOptions<MongoDbSettings> options)
    : MongoDbRepository<MerchantDocument>(new XpnssDbContext(options)), IMerchantsRepository
{
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

        var newMerchant = Mapper.Map(merchantDoc);

        if (newMerchant is null)
            return Result.Fail("Failed to map MerchantDocument to Merchant");

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

        var merchantDoc = await FirstOrDefaultAsync(a =>
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

        var docs = await FindAsync(a => a.UserId == userId && merchantIdList.Contains(a.Id), cancellationToken);

        var merchants = Mapper.Map(docs);

        return Result.Ok(merchants);
    }

    public async Task<Result<IEnumerable<Merchant>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        var docs = await FindAsync(a =>
            a.UserId == userId, cancellationToken);

        var merchants = Mapper.Map(docs);

        return Result.Ok(merchants);
    }
}