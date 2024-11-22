using System.Net;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Application.Commands.BalanceAdjustment;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;


/// <summary>
/// Api endpoint for manually adjusting the balance of an Account.
/// Not completed
/// </summary>
public class AddBalanceAdjustmentEndpoint : BaseEndpoint
{
    public sealed class Endpoint// : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/users/{userId}/accounts/{accountId}/balance",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string accountId,
                        [FromBody] AddBalanceAdjustmentCommand command,
                        [FromServices] IAccountsService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, accountId, command, service, cancellationToken);
                    })
                .Produces<decimal>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Adjust Account Balance")
                .WithName("AdjustAccountBalance")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        string accountId,
        AddBalanceAdjustmentCommand command,
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

        var response = ApiResponse<decimal>.Ok(result.Value);

        return Results.Ok(response);
    }
    public sealed class Validator : AbstractValidator<AddBalanceAdjustmentCommand>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
