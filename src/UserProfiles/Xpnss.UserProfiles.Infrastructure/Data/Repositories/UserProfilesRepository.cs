using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.UserProfiles.Domain.Entities;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Habanerio.Xpnss.UserProfiles.Infrastructure.Data.Documents;
using Habanerio.Xpnss.UserProfiles.Infrastructure.Mappers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.UserProfiles.Infrastructure.Data.Repositories;

public class UserProfilesRepository(
    IMongoDatabase mongoDb,
    ILogger<UserProfilesRepository> logger)
    : MongoDbRepository<UserProfileDocument>(new UserProfileSettingsDbContext(mongoDb)),
        IUserProfilesRepository
{
    private readonly ILogger<UserProfilesRepository> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    public async Task<Result<UserProfile>> AddAsync(
        UserProfile userProfile,
        CancellationToken cancellationToken = default)
    {
        if (userProfile is null)
            return Result.Fail<UserProfile>($"{userProfile} cannot be null");

        try
        {
            var userProfileDocument = InfrastructureMapper.Map(userProfile);

            if (userProfileDocument is null)
                return Result.Fail<UserProfile>("Could not map the UserProfile to a UserProfileDocument");

            await AddDocumentAsync(userProfileDocument, cancellationToken);

            var updatedUserProfile = InfrastructureMapper.Map(userProfileDocument);

            if (updatedUserProfile is null)
                return Result.Fail<UserProfile>("Could not map the UserProfileDocument to a UserProfile");

            return updatedUserProfile;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not Add the UserProfile. " +
                                "ExtUserId: {ExtUserId}; FirstName: {FirstName}; LastName: {LastName}; Email: {Email}",
                                userProfile.ExtUserId,
                                userProfile.FirstName,
                                userProfile.LastName,
                                userProfile.Email);

            return Result.Fail<UserProfile>($"Could not save the UserProfile for User " +
                                            $"{userProfile.FirstName} {userProfile.LastName} ({userProfile.Email})" +
                                            $"{Environment.NewLine}{e.Message}");
        }
    }

    /// <summary>
    /// Gets a user profile by their UserId.
    /// </summary>
    /// <returns>UserProfile?</returns>
    public async Task<Result<UserProfile?>> GetAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId) ||
            userObjectId.Equals(ObjectId.Empty))
            return Result.Fail<UserProfile?>($"{userId} cannot be null or empty");

        var userProfileDocument = await FirstOrDefaultDocumentAsync(a =>
                a.Id.Equals(userObjectId),
            cancellationToken);

        // Do not return a failure if the document is not found
        if (userProfileDocument is null)
            return Result.Ok<UserProfile?>(default);

        var userProfile = InfrastructureMapper.Map(userProfileDocument);

        if (userProfile is null)
            return Result.Fail<UserProfile?>("Failed to map UserProfileDocument to UserProfile");

        return Result.Ok<UserProfile?>(userProfile);
    }

    /// <summary>
    /// Gets a user profile by their Email.
    /// </summary>
    /// <returns>UserProfile?</returns>
    public async Task<Result<UserProfile?>> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail<UserProfile?>($"{email} cannot be null or empty");

        var userProfileDocument = await FirstOrDefaultDocumentAsync(a =>
                a.Email.Equals(email),
            cancellationToken);

        // Do not return a failure if the document is not found
        if (userProfileDocument is null)
            return Result.Ok<UserProfile?>(default);

        var userProfile = InfrastructureMapper.Map(userProfileDocument);

        if (userProfile is null)
        {
            _logger.LogError("Failed to map `{Email}`'s UserProfileDocument to UserProfile. ExtUserId: {ExtUserId}",
                userProfileDocument.Email, userProfileDocument.Email);

            return Result.Fail<UserProfile?>("Failed to map UserProfileDocument to UserProfile");
        }

        return Result.Ok<UserProfile?>(userProfile);
    }

    /// <summary>
    /// Gets a user profile by their External UserId.
    /// </summary>
    /// <returns>UserProfile?</returns>
    public async Task<Result<UserProfile?>> GetByExtIdAsync(
        string extUserId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(extUserId))
            return Result.Fail<UserProfile?>($"{extUserId} cannot be null or empty");

        var userProfileDocument = await FirstOrDefaultDocumentAsync(a =>
                a.ExtUserId.Equals(extUserId),
            cancellationToken);

        // Do not return a failure if the document is not found
        if (userProfileDocument is null)
            return Result.Ok<UserProfile?>(default);

        var userProfile = InfrastructureMapper.Map(userProfileDocument);

        if (userProfile is null)
        {
            _logger.LogError("Failed to map `{Email}` UserProfileDocument to UserProfile. ExtUserId: {ExtUserId}",
                userProfileDocument.Email, extUserId);

            return Result.Fail<UserProfile?>("Failed to map UserProfileDocument to UserProfile");
        }

        return Result.Ok<UserProfile?>(userProfile);
    }

    public async Task<Result<UserProfile>> UpdateAsync(
        UserProfile transaction,
        CancellationToken cancellationToken = default)
    {
        var userProfileDoc = InfrastructureMapper.Map(transaction);

        if (userProfileDoc is null)
            return Result.Fail<UserProfile>("Failed to map UserProfile to UserProfileDocument");

        var saveCount = await UpdateDocumentAsync(userProfileDoc, cancellationToken);

        if (saveCount == 0)
        {
            _logger.LogError("Failed to update UserProfile. " +
                             "ExtUserId: {ExtUserId}; " +
                             "FirstName: {FirstName}; " +
                             "LastName: {LastName}; " +
                             "Email: {Email}",
                transaction.ExtUserId,
                transaction.FirstName,
                transaction.LastName,
                transaction.Email);

            return Result.Fail<UserProfile>("Failed to update UserProfile");
        }

        return transaction;
    }
}