using FluentResults;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Domain.Merchants.Interfaces;

public interface IMerchantsRepository
{
    Task<Result<Merchant>> AddAsync(
        Merchant merchant,
        CancellationToken cancellationToken = default);

    Task<Result<Merchant?>> GetAsync(string userId,
        string merchantId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Merchant>>> GetAsync(
        string userId,
        IEnumerable<ObjectId> merchantIds,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Merchant>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default);
}