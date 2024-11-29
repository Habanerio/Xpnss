using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.UserProfiles.Application.Mappers;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.UserProfiles.Application.Queries;

/// <summary>
/// Responsible for getting a user profile by their Email.
/// </summary>
/// <param name="Email">The (internal) Id of the user</param>
public sealed record GetUserByEmailQuery(string Email) :
    IUserProfilesCommand<Result<UserProfileDto?>>;

public sealed class GetUserByEmailQueryHandler(IUserProfilesRepository repository) :
    IRequestHandler<GetUserByEmailQuery, Result<UserProfileDto?>>
{
    private readonly IUserProfilesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<UserProfileDto?>> Handle(
        GetUserByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var docsResult = await _repository.GetByEmailAsync(request.Email, cancellationToken);

        if (docsResult.IsFailed)
            return Result.Fail(docsResult.Errors);

        var dto = ApplicationMapper.Map(docsResult.Value);

        if (dto is null)
            return Result.Fail("Failed to map UserProfile to UserProfileDto");

        return dto;
    }

    public class Validator : AbstractValidator<GetUserByEmailQuery>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty();
        }
    }
}