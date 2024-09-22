using System.ComponentModel.DataAnnotations;
using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;

/// <summary>
/// Api endpoint for updating account details.
/// </summary>
public class UpdateAccountDetailsEndpoint
{
    public sealed record Request
    {
        [Required]
        public string AccountId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public string DisplayColor { get; set; } = "#ff0000";
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        Request request,
        IAccountsService service,
        CancellationToken cancellationToken)
    {
        var validationResult = new Validator().Validate(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors[0].ErrorMessage);

        var command = new UpdateAccountDetails.Command(
            userId,
            request.AccountId,
            request.Name,
            request.Description,
            request.DisplayColor);

        var result = await service.ExecuteAsync(command, cancellationToken);

        if (result.IsFailed)
            return Results.BadRequest(result.Errors);

        return Results.Ok(result.Value);
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/users/{userId}/accounts/{accountId}",
                async (
                    [FromRoute] string userId,
                    [FromRoute] string accountId,
                    [FromBody] Request request,
                    [FromServices] IAccountsService service,
                    CancellationToken cancellationToken) =>
                {
                    await HandleAsync(userId, request, service, cancellationToken);
                })
                .Produces<string>((int)HttpStatusCode.OK)
                .Produces((int)HttpStatusCode.NotFound)
                .Produces((int)HttpStatusCode.BadRequest)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Update Account Details")
                .WithName("UpdateAccountDetails")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }
}