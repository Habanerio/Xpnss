using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.UserProfiles.Application.Mappers;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.UserProfiles.Application.Queries;

/// <summary>
/// Responsible for getting a user profile by their UserId.
/// </summary>
/// <param name="UserId">The Id (internal) of the user</param>
public sealed record GetUserProfileByIdQuery(string UserId) :
    IUserProfilesQuery<Result<UserProfileDto?>>;

public sealed class GetUserProfileByIdQueryHandler(
    IUserProfilesRepository repository,
    ILogger<GetUserProfileByIdQueryHandler> logger) :
    IRequestHandler<GetUserProfileByIdQuery, Result<UserProfileDto?>>
{
    private readonly ILogger<GetUserProfileByIdQueryHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    private readonly IUserProfilesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<UserProfileDto?>> Handle(
        GetUserProfileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var docsResult = await _repository.GetAsync(request.UserId, cancellationToken);

        if (docsResult.IsFailed)
            return Result.Fail(docsResult.Errors);

        if (docsResult.ValueOrDefault is null)
            return Result.Ok<UserProfileDto?>(default);

        var dto = ApplicationMapper.Map(docsResult.Value);

        if (dto is null)
        {
            _logger.LogError("{GetType}: Failed to map UserProfile {Id} ({Email}) to UserProfileDto",
                nameof(GetType), docsResult.ValueOrDefault.Id, docsResult.ValueOrDefault.Email);

            return Result.Ok<UserProfileDto?>(default);
        }

        return dto;
    }

    public class Validator : AbstractValidator<GetUserProfileByIdQuery>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}