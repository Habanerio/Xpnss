using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.UserProfiles.Application.Mappers;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.UserProfiles.Application.Queries;

/// <summary>
/// Responsible for getting a user profile by their UserId.
/// </summary>
/// <param name="UserId">The Id (internal) of the user</param>
public sealed record GetUserQuery(string UserId) :
    IUserProfilesCommand<Result<UserProfileDto?>>;

public sealed class GetUserQueryHandler(IUserProfilesRepository repository) :
    IRequestHandler<GetUserQuery, Result<UserProfileDto?>>
{
    private readonly IUserProfilesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<UserProfileDto?>> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var docsResult = await _repository.GetAsync(request.UserId, cancellationToken);

        if (docsResult.IsFailed)
            return Result.Fail(docsResult.Errors);

        var dto = ApplicationMapper.Map(docsResult.Value);

        if (dto is null)
            return Result.Fail("Failed to map UserProfile to UserProfileDto");

        return dto;
    }

    public class Validator : AbstractValidator<GetUserQuery>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}