using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.UserProfiles.Application.Commands;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.UserProfiles;

public sealed class CreateUserProfileEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/create",
                    async (
                        [FromBody] CreateUserProfileApiRequest request,
                        [FromServices] IUserProfilesService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(request, service, cancellationToken);
                    })
                .Produces<UserProfileDto>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Create User Profile")
                .WithName("CreateUserProfile")
                .WithTags("UserProfiles")
                .WithOpenApi();
        }

        public static async Task<IResult> HandleAsync(
            CreateUserProfileApiRequest request,
            IUserProfilesService service,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(service);

            var command = new CreateUserProfileCommand(request);

            var result = await service.CommandAsync(command, cancellationToken);

            if (result.IsFailed)
                return BadRequestWithErrors(result.Errors);

            return Results.Ok(result.Value);
        }
    }
}