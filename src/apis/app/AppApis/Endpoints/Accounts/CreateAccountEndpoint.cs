using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Application.Commands.CreateAccount;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.UserProfiles.Application.Queries;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;

/// <summary>
/// Single Endpoint for creating any of the accounts.
/// </summary>
public class CreateAccountEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/accounts",
                    async (
                        [FromRoute] string userId,
                        [FromBody] CreateAccountRequest request,
                        [FromServices] IAccountsService service,
                        [FromServices] IUserProfilesService userProfilesService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, request, service, userProfilesService, cancellationToken);
                    })
                .Produces<AccountDto>()
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Create New Account")
                .WithName("CreateAccount")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateAccountRequest request,
        IAccountsService service,
        IUserProfilesService userProfilesService,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(userProfilesService);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        var getUserCommand = new GetUserProfileByIdQuery(userId);

        var userResult = await userProfilesService.QueryAsync(getUserCommand, cancellationToken);

        if (userResult.IsFailed)
            return BadRequestWithErrors(userResult.Errors);

        if (userResult.Value is null)
            return Results.Unauthorized();

        request = request with { UserId = userId };

        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequestWithErrors(validationResult.Errors);

        var command = new CreateAccountCommand(userId, request);

        var result = await service.CommandAsync(command, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        return Results.Ok(result.Value);
    }

    public sealed class Validator : AbstractValidator<CreateAccountRequest>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.InterestRate).GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100);
        }
    }
}