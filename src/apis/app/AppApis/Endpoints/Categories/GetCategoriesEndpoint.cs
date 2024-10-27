using System.Net;
using Carter;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Categories.CQRS.Queries;
using Habanerio.Xpnss.Modules.Categories.DTOs;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;

public sealed class GetCategoriesEndpoint
{
    public static async Task<IResult> HandleAsync(
        string userId,
        ICategoriesService service,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Results.BadRequest("User Id is required");

        var query = new GetCategories.Query(userId);
        var result = await service.ExecuteAsync(query, cancellationToken);

        if (!result.IsSuccess)
            return Results.BadRequest(result.Errors.Select(x => x.Message));

        if (result.ValueOrDefault is null)
            return Results.NotFound();

        var response = ApiResponse<IEnumerable<CategoryDto>>.Ok(result.Value);

        return Results.Ok(response);
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder builder)
        {
            builder.MapGet("/api/v1/users/{userId}/categories",
                async (
                    [FromRoute] string userId,
                    [FromServices] ICategoriesService service,
                    CancellationToken cancellationToken) =>
                {
                    return await HandleAsync(userId, service, cancellationToken);
                })
                .Produces<ApiResponse<IEnumerable<CategoryDto>>>((int)HttpStatusCode.OK)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Get Categories")
                .WithName("GetCategories")
                .WithTags("Categories")
                .WithOpenApi();
        }
    }
}