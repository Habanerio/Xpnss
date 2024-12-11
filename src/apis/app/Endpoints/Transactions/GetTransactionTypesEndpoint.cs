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
                    () => TransactionEnums.UserTransactionTypes)
                .Produces<Dictionary<string, bool>>()
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get Transaction Types")
                .WithName("GetTransactionTypes")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/transactions/types/all",
                    TransactionEnums.ToDictionary)
                .Produces<IReadOnlyDictionary<int, string>>()
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get All Transaction Types")
                .WithName("GetAllTransactionTypes")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/transactions/types/credits",
                    () => TransactionEnums.CreditTransactionTypes)
                .Produces<IReadOnlyDictionary<int, string>>()
                .WithDisplayName("Get Credit Transaction Types")
                .WithName("GeCreditTransactionTypes")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/transactions/types/debits",
                    () => TransactionEnums.DebitTransactionTypes)
                .Produces<IReadOnlyDictionary<int, string>>()
                .WithDisplayName("Get Debit Transaction Types")
                .WithName("GeDebitTransactionTypes")
                .WithTags("Transactions")
                .WithOpenApi();
        }


    }
}