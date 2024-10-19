using System.ComponentModel.DataAnnotations;
using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Transactions.CQRS.Commands;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public class CreateTransactionEndpoint
{
    public record CreateTransactionRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string AccountId { get; set; }

        public string Description { get; set; } = "";

        public List<TransactionItem> Items { get; set; } = [];

        public Merchant? Merchant { get; set; } = null;

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public string TransactionType { get; set; }
    }

    public record TransactionItem
    {
        public string Id { get; init; } = "";

        public string CategoryId { get; init; } = "";

        public string Description { get; init; } = "";

        public decimal Amount { get; init; }
    }

    public record Merchant
    {
        public string Id { get; init; } = "";

        public string Name { get; init; } = "";

        public string Location { get; init; } = "";
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder builder)
        {
            builder.MapPost("/api/v1/users/{userId}/transactions",
                    async (
                        [FromRoute] string userId,
                        [FromBody] CreateTransactionRequest request,
                        [FromServices] ITransactionsService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, request, service, cancellationToken);
                    }
                )
                .Produces<string>((int)HttpStatusCode.OK)
                .Produces((int)HttpStatusCode.BadRequest)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("New Transaction")
                .WithName("CreateTransaction")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        CreateTransactionRequest request,
        ITransactionsService service,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(service);

        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Results.BadRequest(
                validationResult.Errors
                    .Select(x => x.ErrorMessage));

        var command = new CreateTransaction.Command(
            userId,
            request.AccountId,
            request.Items.Select(i => new TransactionItemDto
            {
                Amount = i.Amount,
                CategoryId = i.CategoryId,
                Description = i.Description
            }),
            request.TransactionDate,
            request.TransactionType,
            request.Description,
            request.Merchant is not null
                ? new MerchantDto
                {
                    Id = request.Merchant.Id,
                    Name = request.Merchant.Name,
                    Location = request.Merchant.Location
                }
                : null);

        var result = await service.ExecuteAsync(command, cancellationToken);

        if (result.IsFailed)
            return Results.BadRequest(result.Errors.Select(x => x.Message));

        var response = ApiResponse<TransactionDto>.Ok(result.Value);

        return Results.Ok(response);
    }

    public sealed class Validator : AbstractValidator<CreateTransactionRequest>
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