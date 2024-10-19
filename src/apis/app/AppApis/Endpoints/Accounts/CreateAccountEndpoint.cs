using System.ComponentModel.DataAnnotations;
using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;

/// <summary>
/// Single Endpoint for creating any of the accounts.
/// </summary>
public class CreateAccountEndpoint
{
    public record CreateAccountRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string AccountType { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public decimal Balance { get; set; }

        public decimal CreditLimit { get; set; }

        public decimal InterestRate { get; set; }

        public decimal OverDraftAmount { get; set; }

        public string DisplayColor { get; set; } = "#ff0000";
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateAccountRequest request,
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
            request.AccountType,
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

        var response = ApiResponse<AccountDto>.Ok(result.Value);

        return Results.Ok(response);
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/accounts",
                    async (
                        [FromRoute] string userId,
                        [FromBody] CreateAccountRequest request,
                        [FromServices] IAccountsService service,
                        CancellationToken cancellationToken) =>
                    {
                        var result = await HandleAsync(userId, request, service, cancellationToken);

                        return result;
                    })
                .Produces<AccountDto>((int)HttpStatusCode.OK)
                .Produces((int)HttpStatusCode.BadRequest)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("New Account")
                .WithName("CreateAccount")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }

    public sealed class Validator : AbstractValidator<CreateAccountRequest>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.AccountType).NotEmpty();
            RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.InterestRate).GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100);
        }
    }
}