using System.Net;
using Carter;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Queries;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;

public class GetAccountEndpoint
{
    public static async Task<IResult> HandleAsync(
        string userId,
        string accountId,
        IAccountsService service,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Results.BadRequest("User Id is required");

        if (string.IsNullOrWhiteSpace(accountId))
            return Results.BadRequest("Account Id is required");

        var query = new GetAccount.Query(userId, accountId);

        var result = await service.ExecuteAsync(query, cancellationToken);

        if (!result.IsSuccess)
            return Results.BadRequest(result.Errors.Select(x => x.Message));

        if (result.ValueOrDefault is null)
            return Results.NotFound();

        var response = ApiResponse<AccountDto>.Ok(result.Value);

        return Results.Ok(response);
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/users/{userId}/accounts/{accountId}",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string accountId,
                        [FromServices] IAccountsService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, accountId, service, cancellationToken);
                    })
                .Produces<ApiResponse<AccountDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get Account")
                .WithName("GetAccount")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }
}