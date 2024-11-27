using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.PayerPayees.Application.Commands.CreatePayerPayee;
using Habanerio.Xpnss.PayerPayees.Application.Queries.GetPayerPayee;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Habanerio.Xpnss.Transactions.Application.Commands;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public sealed class CreateDepositTransaction : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/transactions/deposit",
                    async (
                        [FromRoute] string userId,
                        [FromBody] CreateDepositTransactionRequest request,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, request, transactionsService, payerPayeesService, cancellationToken);
                    }
                )
                .Produces<ApiResponse<PurchaseTransactionDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("New Deposit Transaction")
                .WithName("CreateDepositTransaction")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateDepositTransactionRequest request,
        ITransactionsService transactionsService,
        IPayerPayeesService payerPayeesService,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(transactionsService);
        ArgumentNullException.ThrowIfNull(payerPayeesService);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequestWithErrors(validationResult.Errors);

        //var payerPayeeDto = await DoPayerPayeeAsync(payerPayeesService, userId, request, cancellationToken);

        var payerPayeeId = request.PayerPayee.Id;
        var payerPayeeName = request.PayerPayee.Name;
        var payerPayeeDescription = request.PayerPayee.Description;
        var payerPayeeLocation = request.PayerPayee.Location;

        PayerPayeeDto? payerPayeeDto = null;

        // If the PayerPayeeId is empty, but the PayerPayee Name is not, create a new PayerPayee.
        // If the PayerPayeeId is empty and the PayerPayee Name is empty, then ignore.
        if (string.IsNullOrWhiteSpace(payerPayeeId) && !string.IsNullOrWhiteSpace(payerPayeeName))
        {
            var payerPayeeCommand = new CreatePayerPayeeCommand(
                userId,
                payerPayeeName,
                payerPayeeDescription,
                payerPayeeLocation);

            var payerPayeeResult = await payerPayeesService.CommandAsync(payerPayeeCommand, cancellationToken);

            if (payerPayeeResult is { IsSuccess: true, ValueOrDefault: not null })
            {
                payerPayeeDto = payerPayeeResult.ValueOrDefault;

                request = request with
                {
                    PayerPayee = request.PayerPayee with
                    {
                        Id = payerPayeeDto.Id
                    }
                };
            }
        }
        else if (!string.IsNullOrWhiteSpace(payerPayeeId))
        {
            var getPayerPayeeQuery = new GetPayerPayeeQuery(userId, payerPayeeId);

            var payerPayeeResult = await payerPayeesService.QueryAsync(getPayerPayeeQuery, cancellationToken);

            if (payerPayeeResult is { IsSuccess: true, ValueOrDefault: not null })
            {
                payerPayeeDto = payerPayeeResult.ValueOrDefault;
            }
        }

        var command = new CreateDepositTransactionCommand(userId, request);

        try
        {
            var transactionResult = await transactionsService.CommandAsync(command, cancellationToken);

            if (transactionResult.IsFailed)
                return BadRequestWithErrors(transactionResult.Errors);

            var transactionDto = transactionResult.ValueOrDefault;

            if (transactionDto is null)
                return Results.BadRequest($"An error occurred while trying to return Transaction #{transactionResult.Value.Id}");

            // Assign the PayerPayee, if not null, to the TransactionDto.
            if (payerPayeeDto is not null)
            {
                transactionDto.PayerPayee = payerPayeeDto;
            }

            var response = ApiResponse<DepositTransactionDto>.Ok(transactionDto);

            return Results.Ok(response);
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    public sealed class Validator : AbstractValidator<CreateDepositTransactionRequest>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.TransactionDate).NotEmpty();
            RuleFor(x => x.TransactionType).NotEmpty();
        }
    }

    private static async Task<PayerPayeeDto?> DoPayerPayeeAsync(
        IPayerPayeesService payerPayeesService,
        string userId,
        CreatePurchaseTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        var payerPayeeId = request.PayerPayee.Id;
        var payerPayeeName = request.PayerPayee.Name;
        var payerPayeeDescription = request.PayerPayee.Description;
        var payerPayeeLocation = request.PayerPayee.Description;

        PayerPayeeDto? payerPayeeDto = null;

        // If the PayerPayeeId is empty, but the PayerPayee Name is not, create a new PayerPayee.
        // If the PayerPayeeId is empty and the PayerPayee Name is empty, then ignore.
        if (string.IsNullOrWhiteSpace(payerPayeeId) && !string.IsNullOrWhiteSpace(payerPayeeName))
        {
            var payerPayeeCommand = new CreatePayerPayeeCommand(
            userId,
            payerPayeeName,
            payerPayeeDescription,
            payerPayeeLocation);

            var payerPayeeResult = await payerPayeesService.CommandAsync(payerPayeeCommand, cancellationToken);

            if (payerPayeeResult is { IsSuccess: true, ValueOrDefault: not null })
            {
                payerPayeeDto = payerPayeeResult.ValueOrDefault;

                request = request with
                {
                    PayerPayee = request.PayerPayee with
                    {
                        Id = payerPayeeDto.Id
                    }
                };
            }
        }

        return payerPayeeDto;
    }
}