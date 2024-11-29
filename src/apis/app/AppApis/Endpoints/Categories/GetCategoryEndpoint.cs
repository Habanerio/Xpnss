using System.Net;
using Carter;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Application.Queries.GetCategory;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;

public class GetCategoryEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/users/{userId}/categories/{categoryId}",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string categoryId,
                        [FromServices] ICategoriesService service,
                        CancellationToken cancellationToken = default) =>
                    {
                        return await HandleAsync(userId, categoryId, service, cancellationToken);
                    })
                .Produces<ApiResponse<CategoryDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Get Category")
                .WithName("GetCategory")
                .WithTags("Categories")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
            string userId,
            string categoryId,
            ICategoriesService service,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(service);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors($"{nameof(userId)} cannot be null or empty");

        if (string.IsNullOrWhiteSpace(categoryId))
            return BadRequestWithErrors($"{nameof(categoryId)} cannot be null or empty");

        var query = new GetCategoryQuery(userId, categoryId);
        var result = await service.QueryAsync(query, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        if (result.ValueOrDefault is null)
            return Results.NotFound();

        var response = ApiResponse<CategoryDto>.Ok(result.Value);

        return Results.Ok(response);
    }
}