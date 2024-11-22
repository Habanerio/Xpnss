using System.Net;
using Carter;
using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Merchants.Application.Commands.CreateMerchant;
using Habanerio.Xpnss.Merchants.Domain.Interfaces;
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
            app.MapPost("/api/v1/users/{userId}/transactions",
                    async (
                        [FromRoute] string userId,
                        [FromBody] CreateTransactionCommand command,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IMerchantsService merchantsService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, command, transactionsService, merchantsService, cancellationToken);
                    }
                )
                .Produces<ApiResponse<TransactionDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("NewId Transaction")
                .WithName("CreateTransaction")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateTransactionCommand command,
        ITransactionsService transactionsService,
        IMerchantsService merchantsService,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(transactionsService);
        ArgumentNullException.ThrowIfNull(merchantsService);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequestWithErrors(validationResult.Errors);

        var merchantId = command.Merchant?.Id ?? string.Empty;
        var merchantName = command.Merchant?.Name ?? string.Empty;
        var merchantLocation = command.Merchant?.Location ?? string.Empty;

        // If the MerchantId is empty, but the Merchant Name is not, create a new Merchant.
        // If the MerchantId is empty and the Merchant Name is empty, then ignore.
        if (string.IsNullOrWhiteSpace(merchantId) && !string.IsNullOrWhiteSpace(merchantName))
        {
            var merchantCommand = new CreateMerchantCommand(
                userId,
                merchantName,
                merchantLocation);

            var merchantResult = await merchantsService.CommandAsync(merchantCommand, cancellationToken);

            if (merchantResult.IsSuccess)
            {
                command = command with
                {
                    Merchant = command.Merchant with
                    {
                        Id = merchantResult.Value.Id
                    }
                };
            }
        }

        Result<TransactionDto>? transactionResult;

        try
        {
            transactionResult = await transactionsService.CommandAsync(command, cancellationToken);

            if (transactionResult.IsFailed)
                return BadRequestWithErrors(transactionResult.Errors);

            var transactionDto = transactionResult.ValueOrDefault;

            if (transactionDto is null)
                return Results.BadRequest($"An error occurred while trying to return Transaction #{transactionResult.Value.Id}");

            transactionDto.MerchantName = merchantName;
            transactionDto.MerchantLocation = merchantLocation;

            var response = ApiResponse<TransactionDto>.Ok(transactionDto);

            return Results.Ok(response);
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    public sealed class Validator : AbstractValidator<CreateTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.TransactionDate).NotEmpty();
            RuleFor(x => x.TransactionType).NotEmpty();
        }
    }
}