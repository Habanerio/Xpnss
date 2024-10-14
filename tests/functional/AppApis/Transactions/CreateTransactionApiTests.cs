using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Transactions.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreateTransactionApiTests : BaseFunctionalApisTests,
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_CREATE_TRANSACTION = "/api/v1/users/{userId}/transactions";

    public CreateTransactionApiTests(WebApplicationFactory<Program> factory) :
        base(factory)
    { }

    [Fact]
    public async Task CanCall_CreateTransaction_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var userId = "user-id";
        var request = new CreateTransactionEndpoint.CreateTransactionRequest
        {
            UserId = userId,
            AccountId = ObjectId.GenerateNewId().ToString(),
            TransactionDate = DateTimeOffset.UtcNow,
            TransactionTypeId = (int)TransactionTypes.PURCHASE,
            Items = new List<CreateTransactionEndpoint.TransactionItem>
            {
                new CreateTransactionEndpoint.TransactionItem
                {
                    Amount = 100,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 1 Description"
                    },
                new CreateTransactionEndpoint.TransactionItem
                    {
                    Amount = 200,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 2 Description"
                },
                },
            Merchant = new CreateTransactionEndpoint.Merchant
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Merchant Name",
                Location = "Merchant Location"
            }
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_TRANSACTION
                .Replace("{userId}", USER_ID),
            request);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(apiResponse);
        Assert.True(!string.IsNullOrWhiteSpace(apiResponse.Data));
        Assert.True(apiResponse.IsSuccess);
    }
}