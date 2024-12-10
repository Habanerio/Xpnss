using System.Net;
using Carter;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests.Categories;
using Habanerio.Xpnss.Categories.Application.Commands;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;

/// <summary>
/// Updates a collection of SubCategories that belong to the same Category.
/// </summary>
public class UpdateSubCategoriesEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/users/{userId}/categories/{categoryId}/subs",
                async (
                    [FromRoute] string userId,
                    [FromRoute] string categoryId,
                    [FromBody] List<UpdateSubCategoryApiRequest> request,
                    [FromServices] ICategoriesService service,
                    CancellationToken cancellationToken) =>
                {
                    return await HandleAsync(userId, categoryId, request, service, cancellationToken);
                })
                .Produces<CategoryDto>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Update SubCategory")
                .WithName("UpdateSubCategory")
                .WithTags("Categories")
                .WithOpenApi();
        }

        public static async Task<IResult> HandleAsync(
            string userId,
            string categoryId,
            List<UpdateSubCategoryApiRequest> request,
            ICategoriesService service,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(service);

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequestWithErrors("User Id is required");

            if (string.IsNullOrWhiteSpace(categoryId))
                return BadRequestWithErrors("Category Id is required");

            var subCategories = new List<SubCategoryDto>();

            foreach (var subRequest in request)
            {
                var command = new UpdateSubCategoryCommand(
                    userId,
                    categoryId,
                    subRequest.Id,
                    subRequest.Name,
                    subRequest.Description,
                    subRequest.SortOrder);

                var result = await service.CommandAsync(command, cancellationToken);

                if (result is { IsSuccess: true, ValueOrDefault: not null })
                    subCategories.Add(result.Value);
            }

            return Results.Ok(subCategories);
        }
    }
}