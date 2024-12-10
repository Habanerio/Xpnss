using System.Net;
using Carter;
using Habanerio.Xpnss.Shared.Types;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public sealed class GetTransactionTypesEndpoint : BaseEndpoint
{
    /// <summary>
    /// Returns a dictionary of transaction types.
    /// This is so that any client can have access to the transaction types.
    /// </summary>
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/transactions/types",
                    async (CancellationToken cancellationToken) =>
                    {
                        return TransactionEnums.ToDictionary();
                    })
                .Produces<Dictionary<int, string>>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get Transaction Types")
                .WithName("GetTransactionTypes")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }
}