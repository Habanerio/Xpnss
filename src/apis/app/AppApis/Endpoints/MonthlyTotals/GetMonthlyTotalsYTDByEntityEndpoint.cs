using System.Net;

using Carter;

using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.MonthlyTotals.Application.Queries;
using Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.MonthlyTotals;

public class GetMonthlyTotalsYTDByEntityEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        /// <summary>
        /// Gets all monthly totals for the year to date for a specific entity.
        /// </summary>
        /// <param name="app"></param>
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/users/{userId}/totals/monthly/ytd/{entityType}/{entityId}",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string entityType,
                        [FromRoute] string entityId,
                        [FromServices] IMonthlyTotalsService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, entityType, entityId, service, cancellationToken);
                    })
                .Produces<IEnumerable<MonthlyTotalDto>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get YTD Monthly Totals")
                .WithName("GetYTDMonthlyTotals")
                .WithTags("Totals")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        string entityType,
        string entityId,
        IMonthlyTotalsService service,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(service);

        if (string.IsNullOrWhiteSpace(userId))
            return Results.BadRequest("User Id is required");

        var entityTypeKey = EntityEnums.GetKey(entityType);

        var query = new GetYTDTotalsByEntityQuery(userId, entityTypeKey, entityId);

        var result = await service.QueryAsync(query, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        return Results.Ok(result.Value);
    }
}