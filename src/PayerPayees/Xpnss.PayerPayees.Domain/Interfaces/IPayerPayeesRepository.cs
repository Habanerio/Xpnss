using FluentResults;
using Habanerio.Xpnss.PayerPayees.Domain.Entities;
using MongoDB.Bson;

namespace Habanerio.Xpnss.PayerPayees.Domain.Interfaces;

public interface IPayerPayeesRepository
{
    Task<Result<PayerPayee>> AddAsync(
        PayerPayee payerPayee,
        CancellationToken cancellationToken = default);

    Task<Result<PayerPayee?>> GetAsync(
        string userId,
        string payerPayeeId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<PayerPayee>>> GetAsync(
        string userId,
        IEnumerable<ObjectId> payerPayeeIds,
        CancellationToken cancellationToken = default);

    Task<Result<PayerPayee?>> GetByNameAsync(
        string userId,
        string name,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<PayerPayee>>> ListAsync(
        string userId,
        CancellationToken cancellationToken = default);
}