using System.Net;
using Carter;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.UserProfiles.Application.Commands;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Setup;

public class SetupEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder builder)
        {
            builder.MapPost("/api/v1/setup",
                    async (
                        [FromBody] CreateUserProfileRequest request,
                        [FromServices] IUserProfilesService userProfilesService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(request, userProfilesService, cancellationToken);
                    }
                )
                .Produces<UserProfileDto>((int)HttpStatusCode.OK)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Setup")
                .WithName("Setup")
                .WithTags("Setup")
                .WithOpenApi();
        }

        private static async Task<IResult> HandleAsync(
            CreateUserProfileRequest request,
            IUserProfilesService userProfilesService,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequestWithErrors($"Email is required ({nameof(request.Email)})");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                return BadRequestWithErrors($"First Name is required ({nameof(request.FirstName)})");

            var createUserProfileCommand = new CreateUserProfileCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.ExtUserId);

            var userProfileResult = await userProfilesService.CommandAsync(createUserProfileCommand, cancellationToken);

            if (userProfileResult.IsFailed)
                return BadRequestWithErrors(userProfileResult.Errors[0].Message);

            if (userProfileResult.IsFailed || userProfileResult.ValueOrDefault is null)
                return BadRequestWithErrors(userProfileResult.Errors[0].Message ??
                    $"Could not create a UserProfile for {request.FirstName} ({request.Email})");

            if (string.IsNullOrWhiteSpace(userProfileResult.Value.Id))
                return BadRequestWithErrors($"Could not create a UserProfile for {request.FirstName} ({request.Email})");

            var userProfileDto = userProfileResult.Value;

            if (userProfileDto is null)
                return BadRequestWithErrors($"Could not create a UserProfile for {request.FirstName} ({request.Email})");

            return Results.Ok(userProfileDto);
        }
    }
}