using System.ComponentModel.DataAnnotations;
using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;

/*
public class AdjustBalanceEndpoint
{
    /// <summary>
    /// Adjusts the balance of an account.
    /// This is for when a correction needs to be made to the balance of an account.
    /// It is not for when the balance needs to be updated due to a transaction.
    /// </summary>
    /// <param name="UserId">The Id of the User</param>
    /// <param name="AccountId">The Id of the Account that the Balance belongs to</param>
    /// <param name="NewBalance">The adjusted Balance</param>
    /// <param name="DateChanged">The date for which the adjustment applies to (NOT the date that adjustment was entered into the system)</param>
    /// <param name="Reason">The reason why the adjustment was made</param>
    public sealed record AdjustBalanceRequest(
        [Required] string UserId,
        [Required] string AccountId,
        [Required] decimal NewBalance,
        [Required] DateTime DateChanged,
        string Reason = "");


    public sealed class Validator : AbstractValidator<AdjustBalanceRequest>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        AdjustBalanceRequest request,
        IAccountsService service,
        CancellationToken cancellationToken)
    {
        var validationResult = await new Validator().ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors[0].ErrorMessage);

        var command = new AdjustBalance.Command(
            userId,
            request.AccountId,
            request.NewBalance,
            request.DateChanged,
            request.Reason);

        var result = await service.ExecuteAsync(command, cancellationToken);

        if (result.IsFailed)
            return Results.BadRequest(result.Errors);

        var response = ApiResponse<decimal>.Ok(result.Value);

        return Results.Ok(response);
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/users/{userId}/accounts/{accountId}/balance",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string accountId,
                        [FromBody] AdjustBalanceRequest request,
                        [FromServices] IAccountsService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, request, service, cancellationToken);
                    })
                .Produces((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Adjust Account Balance")
                .WithName("AdjustAccountBalance")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }
}
*/