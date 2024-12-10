using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.UserProfiles.Application.Mappers;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Habanerio.Xpnss.UserProfiles.Application.Queries;

/// <summary>
/// Responsible for getting a user profile by their Email.
/// </summary>
/// <param name="Email">The (internal) Id of the user</param>
public sealed record GetUserProfileByEmailQuery(string Email) :
    IUserProfilesQuery<Result<UserProfileDto?>>;

public sealed class GetUserProfileByEmailQueryHandler(
    IUserProfilesRepository repository,
    ILogger<GetUserProfileByEmailQueryHandler> logger) :
    IRequestHandler<GetUserProfileByEmailQuery, Result<UserProfileDto?>>
{
    private readonly ILogger<GetUserProfileByEmailQueryHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    private readonly IUserProfilesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<UserProfileDto?>> Handle(
        GetUserProfileByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors[0].ErrorMessage);

        var docsResult = await _repository.GetByEmailAsync(request.Email, cancellationToken);

        if (docsResult.IsFailed)
            return Result.Fail(docsResult.Errors);

        var dto = ApplicationMapper.Map(docsResult.Value);

        if (dto is null)
            throw new InvalidCastException(
                $"{nameof(GetType)}: Failed to map UserProfile to UserProfileDto");

        return dto;
    }

    public class Validator : AbstractValidator<GetUserProfileByEmailQuery>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty();
        }
    }
}