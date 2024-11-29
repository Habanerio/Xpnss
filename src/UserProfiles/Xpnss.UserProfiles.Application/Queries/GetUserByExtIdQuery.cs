using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.UserProfiles.Application.Mappers;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.UserProfiles.Application.Queries;

/// <summary>
/// Responsible for getting a user profile by their External UserId.
/// </summary>
/// <param name="ExtUserId">The External Id of the user</param>
public sealed record GetUserByExtIdQuery(string ExtUserId) :
    IUserProfilesCommand<Result<UserProfileDto?>>;

public sealed class GetUserByExtIdQueryHandler(IUserProfilesRepository repository) :
    IRequestHandler<GetUserByExtIdQuery, Result<UserProfileDto?>>
{
    private readonly IUserProfilesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<UserProfileDto?>> Handle(
        GetUserByExtIdQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var docsResult = await _repository.GetByExtIdAsync(request.ExtUserId, cancellationToken);

        if (docsResult.IsFailed)
            return Result.Fail(docsResult.Errors);

        var dto = ApplicationMapper.Map(docsResult.Value);

        if (dto is null)
            return Result.Fail("Failed to map UserProfile to UserProfileDto");

        return dto;
    }

    public class Validator : AbstractValidator<GetUserByExtIdQuery>
    {
        public Validator()
        {
            RuleFor(x => x.ExtUserId).NotEmpty();
        }
    }
}