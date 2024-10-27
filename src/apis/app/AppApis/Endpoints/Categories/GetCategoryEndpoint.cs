using System.Net;
using Carter;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Categories.CQRS.Queries;
using Habanerio.Xpnss.Modules.Categories.DTOs;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;

public class GetCategoryEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        private static async Task<IResult> HandleAsync(
            string userId,
            string parentId,
            ICategoriesService service,
            string childId = "",
            CancellationToken cancellationToken = default)
        {
            var query = new GetCategory.Query(userId, parentId, childId);
            var result = await service.ExecuteAsync(query, cancellationToken);

            if (result.IsFailed)
                return Results.BadRequest(result.Errors.Select(x => x.Message));

            if (result.ValueOrDefault is null)
                return Results.NotFound();

            var response = ApiResponse<CategoryDto>.Ok(result.Value);

            return Results.Ok(response);
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/users/{userId}/categories/{parentId}/{childId}",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string parentId,
                        [FromServices] ICategoriesService service,
                        [FromRoute] string childId = "",
                        CancellationToken cancellationToken = default) =>
                    {
                        return await HandleAsync(userId, parentId, service, childId, cancellationToken);
                    })
                .Produces<ApiResponse<CategoryDto>>((int)HttpStatusCode.OK)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Get Category")
                .WithName("GetCategory")
                .WithTags("Categories")
                .WithOpenApi();
        }
    }
}