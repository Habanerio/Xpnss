using System.Net;
using Carter;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Habanerio.Xpnss.Transactions.Application.Queries.GetTransactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Habanerio.Xpnss.Shared.Requests.Transactions;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public sealed class GetTransactionsEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/transactions/search",
                    async (
                        HttpRequest httpRequest,
                        [FromRoute] string userId,
                        [FromBody] SearchTransactionsRequest request,
                        [FromServices] IAccountsService accountsService,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        CancellationToken cancellationToken) =>
                    {
                        var userTimeZone = httpRequest.Headers["X-User-Timezone"].FirstOrDefault() ?? string.Empty;

                        return await HandleAsync(
                            userId,
                            request,
                            userTimeZone,
                            accountsService,
                            transactionsService,
                            payerPayeesService,
                            cancellationToken);
                    })
                .Produces<IEnumerable<TransactionDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Transactions")
                .WithName("GetUserTransactions")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        SearchTransactionsRequest request,
        string userTimeZone,
        IAccountsService accountsService,
        ITransactionsService transactionsService,
        IPayerPayeesService payerPayeesService,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transactionsService);
        ArgumentNullException.ThrowIfNull(payerPayeesService);

        if (string.IsNullOrWhiteSpace(userId))
            return Results.BadRequest("User Id is required");

        var query = new GetTransactionsQuery(request);

        var result = await transactionsService.QueryAsync(query, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        if (result.ValueOrDefault is null)
            return Results.NotFound();

        return Results.Ok(result.Value);
    }
}