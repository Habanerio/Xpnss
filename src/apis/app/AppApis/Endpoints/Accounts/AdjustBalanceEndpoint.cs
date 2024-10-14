using System.ComponentModel.DataAnnotations;
using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;

public class AdjustBalanceEndpoint
{
    public sealed record AdjustBalanceRequest(
        [Required] string UserId,
        [Required] string AccountId,
        [Required] decimal NewBalance,
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