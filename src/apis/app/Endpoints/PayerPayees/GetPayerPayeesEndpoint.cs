using System.Net;
using Carter;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.PayerPayees.Application.Queries.GetPayerPayees;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.PayerPayees;

public class GetPayerPayeesEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/users/{userId}/payer-payees",
                    async (
                        [FromRoute] string userId,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, payerPayeesService, cancellationToken);
                    })
                .Produces<IEnumerable<PayerPayeeDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Payer Payees")
                .WithName("GetUserPayerPayees")
                .WithTags("Payer Payees")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        IPayerPayeesService payerPayeesService,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(payerPayeesService);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        var query = new GetPayerPayeesQuery(userId);

        var result = await payerPayeesService.QueryAsync(query, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        return Results.Ok(result.Value);
    }
}
