using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Application.Commands;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
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
                    [FromBody] CreateCategoryCommand command,
                    [FromServices] ICategoriesService service,
                    CancellationToken cancellationToken) =>
                {
                    return await HandleAsync(userId, command, service, cancellationToken);
                })
                .Produces<ApiResponse<CategoryDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("NewId Category")
                .WithName("CreateCategory")
                .WithTags("Categories")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateCategoryCommand command,
        ICategoriesService service,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(service);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequestWithErrors(validationResult.Errors);

        var result = await service.CommandAsync(command, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        var response = ApiResponse<CategoryDto>.Ok(result.Value);

        return Results.Ok(response);
    }

    public class Validator : AbstractValidator<CreateCategoryCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}