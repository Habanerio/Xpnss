using System.ComponentModel.DataAnnotations;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Categories.CQRS.Commands;
using Habanerio.Xpnss.Modules.Categories.DTOs;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions.CreateTransactionEndpoint;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;

public class CreateCategoryEndpoint
{
    public sealed record CreateCategoryRequest
    {
        [Required]
        public string UserId { get; init; } = "";

        [Required]
        public string Name { get; init; } = "";

        public string Description { get; init; } = "";

        public string? ParentCategoryId { get; init; } = null;

        public int SortOrder { get; init; } = 99;
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder builder)
        {
            builder.MapPost("/api/v1/users/{userId}/categories",
                async (
                    [FromRoute] string userId,
                    [FromBody] CreateCategoryRequest request,
                    [FromServices] ICategoriesService service,
                    CancellationToken cancellationToken) =>
                {
                    return await HandleAsync(userId, request, service, cancellationToken);
                });
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateCategoryRequest request,
        ICategoriesService service,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(service);

        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Results.BadRequest(
                validationResult.Errors
                    .Select(x => x.ErrorMessage));

        var command = new CreateCategory.Command(
            request.UserId,
            request.Name,
            request.Description,
            request.SortOrder);

        var result = await service.ExecuteAsync(command, cancellationToken);

        if (!result.IsSuccess)
            return Results.BadRequest(result.Errors.Select(x => x.Message));

        var response = ApiResponse<CategoryDto>.Ok(result.Value);

        return Results.Ok(response);
    }

    public class Validator : AbstractValidator<CreateCategoryRequest>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.SortOrder).GreaterThan(0);
        }
    }
}