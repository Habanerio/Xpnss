using System.Net;
using Carter;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.Transactions.DTOs;
using Habanerio.Xpnss.Application.Transactions.Queries.GetTransaction;
using Habanerio.Xpnss.Domain.Merchants.Interfaces;
using Habanerio.Xpnss.Domain.Transactions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public sealed class GetTransactionEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/transactions/{transactionId}",
                    async (
                        HttpRequest httpRequest,
                        [FromRoute] string userId,
                        [FromRoute] string transactionId,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IMerchantsService merchantsService,
                        CancellationToken cancellationToken) =>
                    {
                        var userTimeZone = httpRequest.Headers["X-User-Timezone"].FirstOrDefault() ?? string.Empty;

                        var query = new GetTransactionQuery
                        {
                            UserId = userId,
                            TransactionId = transactionId,
                            TimeZone = userTimeZone
                        };

                        return await HandleAsync(userId, query, transactionsService, merchantsService, cancellationToken);
                    })
                .Produces<ApiResponse<IEnumerable<TransactionDto>>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Transaction")
                .WithName("GetUserTransaction")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        GetTransactionQuery query,
        ITransactionsService transactionsService,
        IMerchantsService merchantsService,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transactionsService);
        ArgumentNullException.ThrowIfNull(merchantsService);

        if (string.IsNullOrWhiteSpace(userId))
            return Results.BadRequest("User Id is required");

        var result = await transactionsService.QueryAsync(query, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        if (result.ValueOrDefault is null)
            return Results.NotFound();

        var response = ApiResponse<TransactionDto>.Ok(result.Value);

        return Results.Ok(response);
    }
}