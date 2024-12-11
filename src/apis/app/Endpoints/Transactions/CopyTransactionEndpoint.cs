using System.Net;
using Carter;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Transactions.Application.Commands;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public class CopyTransactionEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/transactions/{transactionId}/copy",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string transactionId,
                        [FromBody] CopyTransactionRequest[] request,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] ILogger<CopyTransactionEndpoint> logger,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, transactionId, request, transactionsService, logger, cancellationToken);
                    }
                )
                .Produces<IEnumerable<TransactionDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Copy Transaction")
                .WithName("CopyTransactionCommand")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        string transactionId,
        CopyTransactionRequest[] request,
        ITransactionsService transactionsService,
        ILogger<CopyTransactionEndpoint> logger,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(transactionsService);
        ArgumentNullException.ThrowIfNull(logger);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        if (string.IsNullOrWhiteSpace(transactionId))
            return BadRequestWithErrors("Transaction Id is required");

        var command = new CopyTransactionCommand(userId, transactionId, request);

        var results = (await transactionsService.CommandAsync(command, cancellationToken)).ToList();

        if (results.Any(r => r.IsFailed))
            return BadRequestWithErrors(results.SelectMany(e => e.Errors).ToList());

        return Results.Ok(results.Select(r => r.Value));
    }
}