using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
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
                    [FromBody] AddSubCategoriesCommand command,
                    [FromServices] ICategoriesService service,
                    CancellationToken cancellationToken) =>
                {
                    return await HandleAsync(userId, categoryId, command, service, cancellationToken);
                })
                .Produces<ApiResponse<CategoryDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Add SubCategories")
                .WithName("AddSubCategories")
                .WithTags("Categories")
                .WithOpenApi();
        }

        public static async Task<IResult> HandleAsync(
            string userId,
            string categoryId,
            AddSubCategoriesCommand command,
            ICategoriesService service,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(service);

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequestWithErrors("User Id is required");

            if (string.IsNullOrWhiteSpace(categoryId))
                return BadRequestWithErrors("Category Id is required");

            var result = await service.CommandAsync(command, cancellationToken);

            if (result.IsFailed)
                return BadRequestWithErrors(result.Errors);

            var response = ApiResponse<CategoryDto>.Ok(result.Value);

            return Results.Ok(response);
        }
    }
}