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
                    TransactionEnums.ToDictionary)
                .Produces<IReadOnlyDictionary<int, string>>()
                .WithName("GetAllTransactionTypes")
                .WithDisplayName("Get All Transaction Types")
                .WithGroupName("Transactions")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/transactions/types/credits",
                    () => TransactionEnums.CreditTransactionTypes)
                .Produces<IReadOnlyDictionary<int, string>>()
                .WithName("GeCreditTransactionTypes")
                .WithDisplayName("Get Credit Transaction Types")
                .WithDescription("Gets all Credit Transaction Keys")
                .WithGroupName("Transactions")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/transactions/types/debits",
                    () => TransactionEnums.DebitTransactionTypes)
                .Produces<IReadOnlyDictionary<int, string>>()
                .WithDisplayName("Get Debit Transaction Types")
                .WithDescription("Gets all Debit Transaction Keys")
                .WithName("GeDebitTransactionTypes")
                .WithGroupName("Transactions")
                .WithTags("Transactions")
                .WithOpenApi();

            app.MapGet("/api/v1/transactions/types/user",
                    () => TransactionEnums.UserTransactionTypes)
                .Produces<Dictionary<int, string>>()
                .WithDisplayName("Get Transaction Types")
                .WithDescription("Gets all Transaction Keys that are user friendly")
                .WithName("GetTransactionTypes")
                .WithGroupName("Transactions")
                .WithTags("Transactions")
                .WithOpenApi();
        }


    }
}