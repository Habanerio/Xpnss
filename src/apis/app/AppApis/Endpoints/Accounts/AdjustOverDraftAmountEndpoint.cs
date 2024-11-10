using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.Accounts.Commands.AdjustOverDraftAmount;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;


/// <summary>
/// Api endpoint for manually adjusting the balance of an account.
/// </summary>
public class AdjustOverDraftAmountEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/users/{userId}/accounts/{accountId}/overdraft",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string accountId,
                        [FromBody] AdjustOverDraftAmountCommand command,
                        [FromServices] IAccountsService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, command, service, cancellationToken);
                    })
                .Produces<decimal>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Adjust Account Overdraft")
                .WithName("AdjustAccountOverdraft")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        AdjustOverDraftAmountCommand command,
        IAccountsService service,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(service);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        var validationResult = await new Validator().ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequestWithErrors(validationResult.Errors);

        var result = await service.CommandAsync(command, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        var response = ApiResponse<decimal>.Ok(result.Value);

        return Results.Ok(response);
    }

    public sealed class Validator : AbstractValidator<AdjustOverDraftAmountCommand>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.OverDraftAmount).GreaterThanOrEqualTo(0);
        }
    }
}