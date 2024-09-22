using System.ComponentModel.DataAnnotations;
using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;

/// <summary>
/// Single Endpoint for creating any of the accounts.
/// </summary>
public class CreateAccountEndpoint
{
    public record Request
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int AccountTypeId { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public decimal Balance { get; set; }

        public decimal CreditLimit { get; set; }

        public decimal InterestRate { get; set; }

        public decimal OverDraftAmount { get; set; }

        public string DisplayColor { get; set; } = "#ff0000";
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.AccountTypeId).NotEmpty();
            RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.InterestRate).GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100);
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        Request request,
        IAccountsService service,
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

        var command = new CreateAccount.Command(
            request.UserId,
            (AccountType)request.AccountTypeId,
            request.Name,
            request.Description,
            request.Balance,
            request.CreditLimit,
            request.InterestRate,
            request.OverDraftAmount,
            request.DisplayColor);

        var result = await service.ExecuteAsync(command, cancellationToken);

        if (result.IsFailed)
            return Results.BadRequest(result.Errors.Select(x => x.Message));

        var response = ApiResponse<string>.Ok(result.Value);

        return Results.Ok(response);
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/accounts",
                    async (
                        [FromRoute] string userId,
                        [FromBody] Request request,
                        [FromServices] IAccountsService service,
                        CancellationToken cancellationToken) =>
                            await HandleAsync(userId, request, service, cancellationToken))
                .Produces<string>((int)HttpStatusCode.OK)
                .Produces((int)HttpStatusCode.NotFound)
                .Produces((int)HttpStatusCode.BadRequest)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("New Account")
                .WithName("CreateAccount")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }
}