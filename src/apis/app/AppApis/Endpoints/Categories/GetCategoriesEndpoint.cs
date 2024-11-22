using System.Net;
using Carter;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Application.Queries.GetCategories;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;

public sealed class GetCategoriesEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/users/{userId}/categories",
                    async (
                        [FromRoute] string userId,
                        [FromServices] ICategoriesService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, service, cancellationToken);
                    })
                .Produces<ApiResponse<IEnumerable<CategoryDto>>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Get Categories")
                .WithName("GetCategories")
                .WithTags("Categories")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        ICategoriesService service,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(service);
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        var query = new GetCategoriesQuery(userId);
        var result = await service.QueryAsync(query, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        if (result.ValueOrDefault is null)
            return Results.NotFound();

        var response = ApiResponse<IEnumerable<CategoryDto>>.Ok(result.Value);

        return Results.Ok(response);
    }
}