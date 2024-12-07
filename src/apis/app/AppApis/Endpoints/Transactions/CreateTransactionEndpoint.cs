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
                        [FromBody] CreateTransactionRequest request,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, request, transactionsService, payerPayeesService, cancellationToken);
                    }
                )
                .Produces<PurchaseTransactionDto>((int)HttpStatusCode.OK)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("New Transaction")
                .WithName("CreateTransactionCommand")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateTransactionRequest request,
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

        var payerPayeeId = request.PayerPayee?.Id ?? string.Empty;
        var payerPayeeName = request.PayerPayee?.Name ?? string.Empty;
        var payerPayeeDescription = request.PayerPayee?.Description ?? string.Empty;
        var payerPayeeLocation = request.PayerPayee?.Location ?? string.Empty;

        PayerPayeeDto? payerPayeeDto = null;

        if (!string.IsNullOrWhiteSpace(payerPayeeName))
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
                    PayerPayee = new PayerPayeeRequest
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
            return Results.BadRequest(e.Message);
        }
    }

    public sealed class Validator : AbstractValidator<CreateTransactionRequest>
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

                request.PayerPayee = request.PayerPayee with
                {
                    Id = payerPayeeDto.Id
                };

                //request = request with
                //{
                //    PayerPayee = request.PayerPayee with
                //    {
                //        Id = payerPayeeDto.Id
                //    }
                //};
            }
        }

        return payerPayeeDto;
    }
}