using System.Net;
using Carter;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests.Categories;
using Habanerio.Xpnss.Categories.Application.Commands;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;

public class AddSubCategoriesEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/categories/{categoryId}/subcategories",
                async (
                    [FromRoute] string userId,
                    [FromRoute] string categoryId,
                    [FromBody] AddSubCategoriesApiRequest request,
                    [FromServices] ICategoriesService service,
                    CancellationToken cancellationToken) =>
                {
                    return await HandleAsync(userId, categoryId, request, service, cancellationToken);
                })
                .Produces<CategoryDto>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Add SubCategories")
                .WithName("AddSubCategories")
                .WithTags("Categories")
                .WithOpenApi();
        }

        public static async Task<IResult> HandleAsync(
            string userId,
            string categoryId,
            AddSubCategoriesApiRequest request,
            ICategoriesService service,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(service);

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequestWithErrors("User Id is required");

            if (string.IsNullOrWhiteSpace(categoryId))
                return BadRequestWithErrors("Category Id is required");

            var command = new AddSubCategoriesCommand(userId, request);

            var result = await service.CommandAsync(command, cancellationToken);

            if (result.IsFailed)
                return BadRequestWithErrors(result.Errors);

            return Results.Ok(result.Value);
        }
    }
}