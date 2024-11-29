using FluentResults;
using Habanerio.Xpnss.UserProfiles.Domain.Entities;

namespace Habanerio.Xpnss.UserProfiles.Domain.Interfaces;

public interface IUserProfilesRepository
{
    Task<Result<UserProfile>> AddAsync(
        UserProfile userProfile,
        CancellationToken cancellationToken = default);

    Task<Result<UserProfile?>> GetAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<UserProfile?>> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result<UserProfile?>> GetByExtIdAsync(
        string extUserId,
        CancellationToken cancellationToken = default);

    Task<Result<UserProfile>> UpdateAsync(
        UserProfile transaction,
        CancellationToken cancellationToken = default);
}