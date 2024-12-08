using System.Net;

using Carter;

using FluentValidation;

using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.PayerPayees.Application.Commands.CreatePayerPayee;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Habanerio.Xpnss.Transactions.Application.Commands;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public sealed class CreateTransactionEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/transactions/",
                    async (
                        [FromRoute] string userId,
                        [FromBody] CreateTransactionApiRequest request,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        [FromServices] ILogger<CreateTransactionEndpoint> logger,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, request, transactionsService, payerPayeesService, logger, cancellationToken);
                    }
                )
                .Produces<PurchaseTransactionDto>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("New Transaction")
                .WithName("CreateTransactionCommand")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateTransactionApiRequest request,
        ITransactionsService transactionsService,
        IPayerPayeesService payerPayeesService,
        ILogger<CreateTransactionEndpoint> logger,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(transactionsService);
        ArgumentNullException.ThrowIfNull(payerPayeesService);
        ArgumentNullException.ThrowIfNull(logger);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        // Would like to associate the new/existing PayerPayee with the Transaction somehow differently.//
        var payerPayeeName = request.PayerPayee?.Name ?? string.Empty;

        PayerPayeeDto? payerPayeeDto = null;

        if (!string.IsNullOrWhiteSpace(payerPayeeName))
        {
            var payerPayeeCommand = new CreatePayerPayeeCommand(
                userId,
                payerPayeeName);

            var payerPayeeResult = await payerPayeesService
                .CommandAsync(payerPayeeCommand, cancellationToken);

            if (payerPayeeResult is { IsSuccess: true, ValueOrDefault: not null })
            {
                payerPayeeDto = payerPayeeResult.ValueOrDefault;

                request = request with
                {
                    PayerPayee = new PayerPayeeApiRequest
                    {
                        Id = payerPayeeDto.Id,
                        Name = payerPayeeDto.Name,
                        Description = payerPayeeDto.Description,
                        Location = payerPayeeDto.Location
                    }
                };
            }
        }

        var command = new CreateTransactionCommand(request);

        try
        {
            var transactionResult = await transactionsService.CommandAsync(command, cancellationToken);

            if (transactionResult.IsFailed)
                return BadRequestWithErrors(transactionResult.Errors);

            var transactionDto = transactionResult.ValueOrDefault;

            if (transactionDto is null)
                return BadRequestWithErrors(
                    $"An error occurred while trying to return Transaction #{transactionResult.Value.Id}");

            // Assign the PayerPayee, if not null, to the TransactionDto.
            if (payerPayeeDto is not null)
            {
                transactionDto.PayerPayee = payerPayeeDto;
            }

            return Results.Ok(transactionDto);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "An error occurred while trying to create a new Transaction:" +
                                    "\r\n UserId: {UserId}" +
                                    "\r\n AccountId: {AccountId}" +
                                    "\r\n TransactionType: {TransactionType}" +
                                    "\r\n TotalAmount: {TotalAmount}",
                                    request.UserId,
                                    request.AccountId,
                                    request.TransactionType,
                                    request.TotalAmount);
            return Results.BadRequest(e.Message);
        }
    }


}