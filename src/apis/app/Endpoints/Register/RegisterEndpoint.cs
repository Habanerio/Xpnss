using System.Net;
using Carter;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.UserProfiles.Application.Commands;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Setup;

public class RegisterEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder builder)
        {
            builder.MapPost("/api/v1/register",
                    async (
                        [FromBody] CreateUserProfileApiRequest request,
                        [FromServices] IUserProfilesService userProfilesService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(request, userProfilesService, cancellationToken);
                    }
                )
                .Produces<UserProfileDto>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Register")
                .WithName("Register")
                .WithTags("Register")
                .WithOpenApi();
        }

        private static async Task<IResult> HandleAsync(
            CreateUserProfileApiRequest request,
            IUserProfilesService userProfilesService,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequestWithErrors($"Email is required ({nameof(request.Email)})");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                return BadRequestWithErrors($"First Name is required ({nameof(request.FirstName)})");

            var createUserProfileCommand = new CreateUserProfileCommand(request);

            var userProfileResult = 
                await userProfilesService.CommandAsync(createUserProfileCommand, cancellationToken);

            if (userProfileResult.IsFailed)
                return BadRequestWithErrors(userProfileResult.Errors[0].Message);

            if (userProfileResult.IsFailed || userProfileResult.ValueOrDefault is null)
                return BadRequestWithErrors(userProfileResult.Errors[0].Message ??
                    $"Could not create a UserProfile for {request.FirstName} ({request.Email})");

            if (string.IsNullOrWhiteSpace(userProfileResult.Value.Id))
                return BadRequestWithErrors(
                    $"Could not create a UserProfile for {request.FirstName} ({request.Email})");

            var userProfileDto = userProfileResult.Value;

            if (userProfileDto is null)
                return BadRequestWithErrors(
                    $"Could not create a UserProfile for {request.FirstName} ({request.Email})");

            return Results.Ok(userProfileDto);
        }
    }
}