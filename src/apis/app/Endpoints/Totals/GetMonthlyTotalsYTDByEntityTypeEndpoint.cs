//using System.Net;

//using Carter;

//using Habanerio.Xpnss.Shared.DTOs;
//using Habanerio.Xpnss.Shared.Types;
//using Habanerio.Xpnss.Totals.Application.Queries;
//using Habanerio.Xpnss.Totals.Domain.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.MonthlyTotals;

//public class GetMonthlyTotalsYTDByEntityTypeEndpoint : BaseEndpoint
//{
//    public sealed class Endpoint : ICarterModule
//    {
//        /// <summary>
//        /// Gets all monthly totals for the year to date for a specific entity.
//        /// </summary>
//        /// <param name="app"></param>
//        public void AddRoutes(IEndpointRouteBuilder app)
//        {
//            app.MapGet("/api/v1/users/{userId}/totals/monthly/ytd/{entityType}",
//                    async (
//                        [FromRoute] string userId,
//                        [FromRoute] string entityType,
//                        [FromServices] IMonthlyTotalsService service,
//                        CancellationToken cancellationToken) =>
//                    {
//                        return await HandleAsync(userId, entityType, service, cancellationToken);
//                    })
//                .Produces<IEnumerable<MonthlyTotalDto>>((int)HttpStatusCode.OK)
//                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
//                .Produces((int)HttpStatusCode.NotFound)
//                .WithDisplayName("Get YTD Monthly Totals By Entity Type")
//                .WithDescription("Gets all Monthly Totals for a specific Entity Type. For example, all Accounts Monthly Totals")
//                .WithName("GetYTDMonthlyTotals")
//                .WithTags("Totals")
//                .WithOpenApi();
//        }
//    }

//    public static async Task<IResult> HandleAsync(
//        string userId,
//        string entityType,
//        IMonthlyTotalsService service,
//        CancellationToken cancellationToken)
//    {
//        ArgumentNullException.ThrowIfNull(service);

//        if (string.IsNullOrWhiteSpace(userId))
//            return Results.BadRequest("User Id is required");

//        var entityTypeKey = EntityEnums.GetKey(entityType);

//        var query = new GetYTDTotalsByEntityQuery(userId, entityTypeKey);

//        var result = await service.QueryAsync(query, cancellationToken);

//        if (result.IsFailed)
//            return BadRequestWithErrors(result.Errors);

//        return Results.Ok(result.Value);
//    }
//}