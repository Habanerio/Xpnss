using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.IntegrationEvents.UserProfiles;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.UserProfiles.Domain.Entities;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.UserProfiles.Application.Commands;

public sealed record CreateUserProfileCommand(
    CreateUserProfileApiRequest Request) :
    IUserProfilesCommand<Result<UserProfileDto>>;

public class CreateUserProfileCommandHandler(
    IUserProfilesRepository repository,
    IMediator mediator,
    ILogger<CreateUserProfileCommandHandler> logger) :
    IRequestHandler<CreateUserProfileCommand, Result<UserProfileDto>>
{
    private readonly ILogger<CreateUserProfileCommandHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    private readonly IUserProfilesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<UserProfileDto>> Handle(
        CreateUserProfileCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var doesUserExistResult = await _repository.GetByEmailAsync(request.Email, cancellationToken);

        if (doesUserExistResult.IsFailed)
        {
            _logger.LogError("{GetType}: Could not check if the User Profile already exists for {Email}",
                nameof(GetType), request.Email);

            return Result.Fail($"Could not check if the User Profile already exists for {request.Email}");
        }

        if (doesUserExistResult.ValueOrDefault is not null)
        {
            _logger.LogWarning("{GetType}: User Profile already exists for {Email}",
                nameof(GetType), request.Email);

            return Result.Fail($"User Profile already exists for {request.Email}");
        }

        var extUserId = request.ExtUserId.Trim();
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var email = request.Email.Trim();

        var userProfile = UserProfile.New(extUserId, firstName, lastName, email, request.DefaultCurrency);

        var result = await _repository.AddAsync(userProfile, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
        {
            _logger.LogError("{GetType}: Could not save '{FirstName}' User Profile ({Email})",
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

        var userProfileCreatedEvent = new UserProfileCreatedIntegrationEvent(userProfileDto.Id);

        await _mediator.Publish(userProfileCreatedEvent, cancellationToken);

        return userProfileDto;
    }

    public class Validator : AbstractValidator<CreateUserProfileApiRequest>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}