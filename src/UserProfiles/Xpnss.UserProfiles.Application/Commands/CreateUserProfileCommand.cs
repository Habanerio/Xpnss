using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.UserProfiles.Domain.Entities;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.UserProfiles.Application.Commands;

public sealed record CreateUserProfileCommand(
    string Email,
    string FirstName,
    string LastName,
    string ExtUserId) :
    IUserProfilesCommand<Result<UserProfileDto>>;

public class CreateUserProfileCommandHandler(
    IUserProfilesRepository repository,
    ILogger<CreateUserProfileCommandHandler> logger) :
    IRequestHandler<CreateUserProfileCommand, Result<UserProfileDto>>
{
    private readonly ILogger<CreateUserProfileCommandHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    private readonly IUserProfilesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<UserProfileDto>> Handle(
        CreateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var extUserId = request.ExtUserId.Trim();
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var email = request.Email.Trim();

        var userProfile = UserProfile.New(extUserId, firstName, lastName, email);

        var result = await _repository.AddAsync(userProfile, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
        {
            _logger.LogError("{GetType}: Could not save the {FirstName} User Profile ({Email})",
                nameof(GetType), firstName, email);

            return Result.Fail(result.Errors?[0].Message ??
                               $"Could not save the {firstName} User Profile ({email})");
        }

        var userProfileDto = Mappers.ApplicationMapper.Map(result.ValueOrDefault);

        if (userProfileDto is null)
        {
            _logger.LogError("{GetType}: Failed to map {FirstName} UserProfile ({Email}) to UserProfileDto",
                nameof(GetType), firstName, email);

            return Result.Fail($"{nameof(GetType)}: Failed to map {firstName} UserProfile ({email}) to UserProfileDto");
        }

        return userProfileDto;
    }

    public class Validator : AbstractValidator<CreateUserProfileCommand>
    {
        public Validator()
        {
            //RuleFor(x => x.ExtUserId).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}