using System.ComponentModel.DataAnnotations;
using System.Net;
using Carter;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Transactions.CQRS.Queries;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public class GetTransactionsEndpoint
{
    public sealed record GetTransactionsRequest
    {
        [Required]
        public string UserId { get; set; }

        public string AccountId { get; set; } = "";

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        //public int Page { get; set; } = 1;

        //public int PageSize { get; set; } = 10;
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        GetTransactionsRequest request,
        ITransactionsService service,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Results.BadRequest("User Id is required");

        var query = new GetTransactions.Query(
            userId,
            request.AccountId,
            FromDate: request.StartDate,
            ToDate: request.EndDate);

        var result = await service.ExecuteAsync(query, cancellationToken);

        if (!result.IsSuccess)
            return Results.BadRequest(result.Errors.Select(x => x.Message));

        if (result.ValueOrDefault is null)
            return Results.NotFound();

        var response = ApiResponse<IEnumerable<TransactionDto>>.Ok(result.Value);

        return Results.Ok(response);
    }

    public sealed class Endoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/transactions/search",
                    async (
                        [FromRoute] string userId,
                        [FromBody] GetTransactionsRequest request,
                        [FromServices] ITransactionsService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, request, service, cancellationToken);
                    })
                .Produces<ApiResponse<IEnumerable<TransactionDto>>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Transactions")
                .WithName("GetUserTransactions")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }
}