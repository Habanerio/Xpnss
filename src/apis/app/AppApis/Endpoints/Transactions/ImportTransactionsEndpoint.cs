//using System.Net;
//using Carter;
//using Habanerio.Xpnss.Apis.App.AppApis.Models;
//using Habanerio.Xpnss.Application.DTOs;
//using Habanerio.Xpnss.Application.Requests;
//using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
//using Habanerio.Xpnss.Transactions.Domain.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

//public sealed class ImportTransactionsEndpoint : BaseEndpoint
//{
//    public sealed class Endpoint : ICarterModule
//    {
//        public void AddRoutes(IEndpointRouteBuilder app)
//        {
//            app.MapPost("/api/v1/users/{userId}/transactions/import",
//                    async (
//                        HttpRequest httpRequest,
//                        [FromRoute] string userId,
//                        [FromBody] ImportTransactionsRequest apiRequest,
//                        [FromServices] ITransactionsService transactionsService,
//                        [FromServices] IPayerPayeesService payerPayeesService,
//                        CancellationToken cancellationToken) =>
//                    {
//                        var userTimeZone = httpRequest.Headers["X-User-Timezone"].FirstOrDefault() ?? string.Empty;

//                        return await HandleAsync(userId, apiRequest, userTimeZone, transactionsService, payerPayeesService, cancellationToken);
//                    })
//                .Produces<ApiResponse<IEnumerable<TransactionDto>>>((int)HttpStatusCode.OK)
//                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
//                .Produces((int)HttpStatusCode.NotFound)
//                .WithDisplayName("Import Transactions")
//                .WithName("ImportTransactions")
//                .WithTags("Transactions")
//                .WithOpenApi();
//        }
//    }
//}