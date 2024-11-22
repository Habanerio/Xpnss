using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Merchants.Domain.Interfaces;
using Habanerio.Xpnss.Transactions.Application.Commands;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public class UpdateTransactionEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/users/{userId}/transactions/{transactionId}",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string transactionId,
                        [FromBody] UpdateTransactionCommand command,
                        [FromServices] ITransactionsService transactionsService,
                        [FromServices] IMerchantsService merchantsService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, transactionId, command, transactionsService, merchantsService, cancellationToken);
                    })
                .Produces<ApiResponse<TransactionDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Update Transaction")
                .WithName("UpdateTransaction")
                .WithTags("Transactions")
                .WithOpenApi();
        }

        public static async Task<IResult> HandleAsync(
            string userId,
            string transactionId,
            UpdateTransactionCommand command,
            ITransactionsService transactionsService,
            IMerchantsService merchantsService,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(transactionsService);
            ArgumentNullException.ThrowIfNull(merchantsService);

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequestWithErrors("User Id is required");

            if (string.IsNullOrWhiteSpace(transactionId))
                return BadRequestWithErrors("Transaction Id is required");

            var validationResult = await new Validator().ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequestWithErrors(validationResult.Errors);

            var result = await transactionsService.CommandAsync(command, cancellationToken);

            if (result.IsFailed)
                return BadRequestWithErrors(result.Errors);

            return Results.Ok(result.Value);
        }

        public sealed class Validator : AbstractValidator<UpdateTransactionCommand>
        {
            public Validator()
            {
                RuleFor(x => x).NotNull();
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.TransactionId).NotEmpty();
                RuleFor(x => x.TransactionDate).NotEmpty();
                RuleFor(x => x.TransactionType).NotEmpty();
            }
        }
    }
}