using System.Net;
using Carter;
using Habanerio.Xpnss.Transactions.Application.Commands;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public sealed class DeleteTransactionEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/v1/users/{userId}/transactions/{transactionId}",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string transactionId,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] ILogger<DeleteTransactionEndpoint> logger,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(
                            userId,
                            transactionId,
                            transactionsService,
                            logger,
                            cancellationToken);
                    }
                )
                .Produces<bool>()
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Delete Transaction")
                .WithName("DeleteTransaction")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        string transactionId,
        ITransactionsService transactionsService,
        ILogger<DeleteTransactionEndpoint> logger,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(transactionsService);
        ArgumentNullException.ThrowIfNull(logger);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        if (string.IsNullOrWhiteSpace(transactionId))
            return BadRequestWithErrors("Transaction Id is required");

        var command = new DeleteTransactionCommand(userId, transactionId);

        var result = await transactionsService.CommandAsync(command, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        return Results.Ok(true);
    }
}