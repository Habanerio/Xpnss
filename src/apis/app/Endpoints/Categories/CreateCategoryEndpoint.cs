using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Categories.Application.Commands;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests.Categories;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;

public class CreateCategoryEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/categories",
                async (
                    [FromRoute] string userId,
                    [FromBody] CreateCategoryApiRequest request,
                    [FromServices] ICategoriesService service,
                    CancellationToken cancellationToken) =>
                {
                    return await HandleAsync(userId, request, service, cancellationToken);
                })
                .Produces<CategoryDto>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Create New Category")
                .WithName("CreateCategory")
                .WithTags("Categories")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateCategoryApiRequest request,
        ICategoriesService service,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(service);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequestWithErrors(validationResult.Errors);

        var command = new CreateCategoryCommand(userId, request);

        var result = await service.CommandAsync(command, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        return Results.Ok(result.Value);
    }

    public class Validator : AbstractValidator<CreateCategoryApiRequest>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}