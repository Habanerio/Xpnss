using System.Net;
using Carter;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.UserProfiles.Application.Queries;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.UserProfiles;

public sealed class GetUserProfileEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/users/{userId}",
                    async (
                        [FromRoute] string userId,
                        [FromServices] IUserProfilesService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, service, cancellationToken);
                    })
                .Produces<UserProfileDto>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Get User Profile")
                .WithName("GetUserProfile")
                .WithTags("UserProfiles")
                .WithOpenApi();
        }

        public static async Task<IResult> HandleAsync(
            string userId,
            IUserProfilesService service,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequestWithErrors("User Id is required");

            if (service is null)
                return BadRequestWithErrors("User Profile Service is required");

            var query = new GetUserQuery(userId);

            var userResult = await service.QueryAsync(query, cancellationToken);

            if (userResult.IsFailed)
                return BadRequestWithErrors(userResult.Errors);

            if (userResult.ValueOrDefault is null)
                return Results.NotFound();

            return Results.Ok(new ApiResponse<UserProfileDto>(userResult.Value));
        }
    }
}