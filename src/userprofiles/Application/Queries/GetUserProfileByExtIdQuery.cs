using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.UserProfiles.Application.Mappers;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.UserProfiles.Application.Queries;

/// <summary>
/// Responsible for getting a user profile by their External UserId.
/// </summary>
/// <param name="ExtUserId">The External Id of the user</param>
public sealed record GetUserProfileByExtIdQuery(string ExtUserId) :
    IUserProfilesQuery<Result<UserProfileDto?>>;

public sealed class GetUserProfileByExtIdQueryHandler(
    IUserProfilesRepository repository,
    ILogger<GetUserProfileByExtIdQueryHandler> logger) :
    IRequestHandler<GetUserProfileByExtIdQuery, Result<UserProfileDto?>>
{
    private readonly ILogger<GetUserProfileByExtIdQueryHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    private readonly IUserProfilesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<UserProfileDto?>> Handle(
        GetUserProfileByExtIdQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var docResult = await _repository.GetByExtIdAsync(request.ExtUserId, cancellationToken);

        if (docResult.IsFailed)
            return Result.Fail(docResult.Errors);

        if (docResult.ValueOrDefault is null)
            return Result.Ok<UserProfileDto?>(null);

        var dto = ApplicationMapper.Map(docResult.Value);

        if (dto is null)
            throw new InvalidCastException(
                $"{nameof(GetType)}: Failed to map UserProfile to UserProfileDto");

        return dto;
    }

    public class Validator : AbstractValidator<GetUserProfileByExtIdQuery>
    {
        public Validator()
        {
            RuleFor(x => x.ExtUserId).NotEmpty();
        }
    }
}