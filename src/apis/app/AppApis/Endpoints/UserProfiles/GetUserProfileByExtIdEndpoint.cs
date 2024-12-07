using System.Net;
using Carter;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.UserProfiles.Application.Queries;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.UserProfiles;

public sealed class GetUserProfileByExtIdEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/users/ext/{extUserId}",
                    async (
                        [FromRoute] string extUserId,
                        [FromServices] IUserProfilesService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(extUserId, service, cancellationToken);
                    })
                .Produces<UserProfileDto>((int)HttpStatusCode.OK)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Get User Profile By Ext Id")
                .WithName("GetUserProfileByExtId")
                .WithTags("UserProfiles")
                .WithOpenApi();
        }

        public static async Task<IResult> HandleAsync(
            string extUserId,
            IUserProfilesService service,
            CancellationToken cancellationToken)
        {
            if (service is null)
                return BadRequestWithErrors("User Profile Service is required");

            if (string.IsNullOrWhiteSpace(extUserId))
                return BadRequestWithErrors("Ext User Id is required");

            var query = new GetUserProfileByExtIdQuery(extUserId);

            var userResult = await service.QueryAsync(query, cancellationToken);

            if (userResult.IsFailed)
                return BadRequestWithErrors(userResult.Errors);

            if (userResult.ValueOrDefault is null)
                return Results.NotFound();

            return Results.Ok(userResult.Value);
        }
    }
}