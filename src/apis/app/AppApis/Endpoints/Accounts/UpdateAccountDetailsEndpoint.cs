using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.Accounts.Commands.UpdateAccountDetails;
using Habanerio.Xpnss.Application.Accounts.DTOs;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;

/// <summary>
/// Api endpoint for updating account details.
/// </summary>
public class UpdateAccountDetailsEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/users/{userId}/accounts/{accountId}",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string accountId,
                        [FromBody] UpdateAccountDetailsCommand command,
                        [FromServices] IAccountsService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, accountId, command, service, cancellationToken);
                    })
                .Produces<ApiResponse<AccountDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Update Account Details")
                .WithName("UpdateAccountDetails")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        string accountId,
        UpdateAccountDetailsCommand command,
        IAccountsService service,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(service);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        if (string.IsNullOrWhiteSpace(accountId))
            return BadRequestWithErrors("Account Id is required");

        var validationResult = await new Validator().ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequestWithErrors(validationResult.Errors);

        var result = await service.CommandAsync(command, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        var response = new ApiResponse<AccountDto>(result.Value);

        return Results.Ok(response);
    }

    public sealed class Validator : AbstractValidator<UpdateAccountDetailsCommand>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}