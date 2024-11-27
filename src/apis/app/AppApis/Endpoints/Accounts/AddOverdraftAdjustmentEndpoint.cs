using System.Net;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Application.Commands.OverdraftAdjustment;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;


/// <summary>
/// Api endpoint for manually adjusting the balance of an Account.
/// Not completed
/// </summary>
public class AddOverdraftAdjustmentEndpoint : BaseEndpoint
{
    public sealed class Endpoint// : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/users/{userId}/accounts/{accountId}/overdraft",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string accountId,
                        [FromBody] AddOverdraftAdjustmentCommand command,
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
        AddOverdraftAdjustmentCommand command,
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

    public sealed class Validator : AbstractValidator<AddOverdraftAdjustmentCommand>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.OverdraftAmount).GreaterThanOrEqualTo(0);
        }
    }
}
