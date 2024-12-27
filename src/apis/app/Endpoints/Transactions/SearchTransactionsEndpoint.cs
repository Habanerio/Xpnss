using System.Net;
using Carter;
using Habanerio.Xpnss.Accounts.Application.Queries.GetAccounts;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Categories.Application.Queries.GetCategories;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.PayerPayees.Application.Queries.GetPayerPayees;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.Responses.Transactions;
using Habanerio.Xpnss.Transactions.Application.Queries.SearchTransactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;

public sealed class SearchTransactionsEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/{userId}/transactions/search",
                    async (
                        HttpRequest httpRequest,
                        [FromRoute] string userId,
                        [FromBody] SearchTransactionsRequest request,
                        [FromServices] IAccountsService accountsService,
                        [FromServices] ICategoriesService categoriesService,
                        [FromServices] IPayerPayeesService payerPayeesService,
                        [FromServices] ITransactionsService transactionsService,

                        CancellationToken cancellationToken) =>
                    {
                        var userTimeZone = httpRequest.Headers["X-User-Timezone"].FirstOrDefault() ?? string.Empty;

                        return await HandleAsync(
                            userId,
                            request,
                            userTimeZone,
                            accountsService,
                            categoriesService,
                            payerPayeesService,
                            transactionsService,
                            cancellationToken);
                    })
                .Produces<TransactionsSearchResponse>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Get User Transactions")
                .WithName("GetUserTransactions")
                .WithTags("Transactions")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        SearchTransactionsRequest request,
        string userTimeZone,
        IAccountsService accountsService,
        ICategoriesService categoriesService,
        IPayerPayeesService payerPayeesService,
        ITransactionsService transactionsService,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transactionsService);
        ArgumentNullException.ThrowIfNull(payerPayeesService);

        if (string.IsNullOrWhiteSpace(userId))
            return Results.BadRequest("User Id is required");

        var query = new SearchTransactionsQuery(request);

        var result = await transactionsService.QueryAsync(query, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        if (result.ValueOrDefault is null)
            return Results.NotFound();

        var searchResponse = result.Value;

        if (!searchResponse.Transactions.Any())
            return Results.Ok(searchResponse);


        var accountsQuery = new GetAccountsQuery(userId);
        var accounts = (await accountsService.QueryAsync(accountsQuery, cancellationToken))
            .ValueOrDefault?.ToArray() ?? [];

        var categoriesQuery = new GetCategoriesQuery(userId);
        var categories = (await categoriesService.QueryAsync(categoriesQuery, cancellationToken))
            .ValueOrDefault?.ToArray() ?? [];

        var payerPayeesQuery = new GetPayerPayeesQuery(userId);
        var payerPayees = (await payerPayeesService.QueryAsync(payerPayeesQuery, cancellationToken))
            .ValueOrDefault?.ToArray() ?? [];

        var transactions = searchResponse.Transactions.ToList();

        foreach (var transaction in transactions)
        {
            if (transaction.Account is not null)
            {
                var accountDto = accounts?
                        .FirstOrDefault(a =>
                            a.Id == transaction.Account?.Key && !a.IsDeleted) ??
                        default;


                transaction.Account = accountDto != null
                    ? new KeyValuePair<string, string>(accountDto.Id, accountDto.Name)
                    : default;
            }

            if (transaction.Category is not null)
            {
                var categoryDto = categories?
                        .FirstOrDefault(a =>
                            a.Id == transaction.Category?.Key) ??
                                 default;

                transaction.Category = categoryDto != null
                    ? new KeyValuePair<string, string>(categoryDto.Id, categoryDto.Name)
                    : default;


                if (categoryDto is not null && transaction.SubCategory is not null)
                {
                    var subCategoryDto = categoryDto?.SubCategories
                        .FirstOrDefault(a =>
                                                 a.Id == transaction.SubCategory?.Key) ??
                                         default;


                    transaction.SubCategory = subCategoryDto != null
                        ? new KeyValuePair<string, string>(subCategoryDto.Id, subCategoryDto.Name)
                        : default;
                }
            }

            if (transaction.PayerPayee is not null)
            {
                var payerPayeeDto = payerPayees?
                        .FirstOrDefault(a =>
                                            a.Id == transaction.PayerPayee?.Key) ??
                                    default;


                transaction.PayerPayee = payerPayeeDto != null
                    ? new KeyValuePair<string, string>(payerPayeeDto.Id, payerPayeeDto.Name)
                    : default;
            }
        }

        return Results.Ok(searchResponse);
    }
}