using System.Net;
using Carter;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Habanerio.Xpnss.Transactions.Application.Queries.GetTransaction;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Habanerio.Xpnss.Shared.DTOs.Transactions;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public sealed class GetTransactionEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        /// <summary>
        /// Adds the HTTPGET endpoint routes for each transaction type.
        /// This is so that a known type is returned to the client.
        /// </summary>
        /// <param name="app"></param>
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            //TODO: I should be able to iterate over all the transaction types and create the endpoints dynamically (generics or reflection)

            app.MapGet("/api/v1/users/{userId}/transactions/{transactionId}",
                    async (
                        HttpRequest httpRequest,
                        [FromRoute] string userId,
                        [FromRoute] string transactionId,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        CancellationToken cancellationToken) =>
                    {
                        var userTimeZone = httpRequest.Headers["X-User-Timezone"].FirstOrDefault() ?? string.Empty;

                        return await HandleAsync<TransactionDto>(userId, transactionId, userTimeZone, transactionsService, payerPayeesService, cancellationToken);
                    })
                .Produces<IEnumerable<TransactionDto>>()
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Transaction")
                .WithDescription("Gets a generic transaction DTO")
                .WithName("GetUserTransaction")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/users/{userId}/transactions/{transactionId}/deposit",
                    async (
                        HttpRequest httpRequest,
                        [FromRoute] string userId,
                        [FromRoute] string transactionId,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        CancellationToken cancellationToken) =>
                    {
                        var userTimeZone = httpRequest.Headers["X-User-Timezone"].FirstOrDefault() ?? string.Empty;

                        return await HandleAsync<DepositTransactionDto>(userId, transactionId, userTimeZone, transactionsService, payerPayeesService, cancellationToken);
                    })
                .Produces<IEnumerable<DepositTransactionDto>>()
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Deposit Transaction")
                .WithDescription("Gets a deposit transaction DTO")
                .WithName("GetUserDepositTransaction")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/users/{userId}/transactions/{transactionId}/payment",
                    async (
                        HttpRequest httpRequest,
                        [FromRoute] string userId,
                        [FromRoute] string transactionId,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        CancellationToken cancellationToken) =>
                    {
                        var userTimeZone = httpRequest.Headers["X-User-Timezone"].FirstOrDefault() ?? string.Empty;

                        return await HandleAsync<PaymentTransactionDto>(userId, transactionId, userTimeZone, transactionsService, payerPayeesService, cancellationToken);
                    })
                .Produces<IEnumerable<PaymentTransactionDto>>()
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Payment Transaction")
                .WithDescription("Gets a payment transaction DTO")
                .WithName("GetUserPaymentTransaction")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/users/{userId}/transactions/{transactionId}/purchases",
                    async (
                        HttpRequest httpRequest,
                        [FromRoute] string userId,
                        [FromRoute] string transactionId,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        CancellationToken cancellationToken) =>
                    {
                        var userTimeZone = httpRequest.Headers["X-User-Timezone"].FirstOrDefault() ?? string.Empty;

                        return await HandleAsync<PurchasesTransactionDto>(userId, transactionId, userTimeZone, transactionsService, payerPayeesService, cancellationToken);
                    })
                .Produces<IEnumerable<PurchasesTransactionDto>>()
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Purchases Transaction")
                .WithDescription("Gets a purchases transaction DTO")
                .WithName("GetUserPurchasesTransaction")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/users/{userId}/transactions/{transactionId}/withdrawal",
                    async (
                        HttpRequest httpRequest,
                        [FromRoute] string userId,
                        [FromRoute] string transactionId,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        CancellationToken cancellationToken) =>
                    {
                        var userTimeZone = httpRequest.Headers["X-User-Timezone"].FirstOrDefault() ?? string.Empty;

                        return await HandleAsync<WithdrawalTransactionDto>(userId, transactionId, userTimeZone, transactionsService, payerPayeesService, cancellationToken);
                    })
                .Produces<IEnumerable<WithdrawalTransactionDto>>()
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Withdrawal Transaction")
                .WithDescription("Gets a withdrawal transaction DTO")
                .WithName("GetUserWithdrawalTransaction")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync<TDto>(
        string userId,
        string transactionId,
        string userTimeZone,
        ITransactionsService transactionsService,
        IPayerPayeesService payerPayeesService,
        CancellationToken cancellationToken = default) where TDto : TransactionDto
    {
        ArgumentNullException.ThrowIfNull(transactionsService);
        ArgumentNullException.ThrowIfNull(payerPayeesService);

        if (string.IsNullOrWhiteSpace(userId))
            return Results.BadRequest("User Id is required");

        var query = new GetTransactionQuery(userId, transactionId, userTimeZone);

        var result = await transactionsService.QueryAsync(query, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        if (result.ValueOrDefault is null)
            return Results.NotFound();

        if (result.ValueOrDefault is not TDto dto)
            return BadRequestWithErrors($"Transaction is a '{result.ValueOrDefault.GetType()}', and not of expected type '{typeof(TDto)}'.");

        return Results.Ok(dto);
    }


}