using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Transactions.Common;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
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
        var request = new CreateTransactionEndpoint.CreateTransactionRequest
        {
            UserId = USER_ID,
            AccountId = ObjectId.GenerateNewId().ToString(),
            TransactionDate = DateTime.Now,
            TransactionType = TransactionTypes.PURCHASE.ToString(),
            Items = new List<CreateTransactionEndpoint.TransactionItem>
            {
                new()
                {
                    Amount = 100,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 1 Description"
                    },
                new()
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
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<TransactionDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        var actualDto = Assert.IsType<TransactionDto>(apiResponse.Data);
        Assert.True(!actualDto.Id.Equals(ObjectId.Empty.ToString()));
        Assert.Equal(request.UserId, actualDto.UserId);
        Assert.Equal(request.AccountId, actualDto.AccountId);
        Assert.Equal(DateTime.UtcNow.Date, actualDto.TransactionDate.Date);
        Assert.Equal(request.TransactionType, actualDto.TransactionType);

        Assert.Equal(request.Items.Count, actualDto.Items.Count);

        Assert.NotNull(actualDto.Merchant);
        Assert.Equal(request.Merchant.Id, actualDto.Merchant.Id);
        Assert.Equal(request.Merchant.Name, actualDto.Merchant.Name);
        Assert.Equal(request.Merchant.Location, actualDto.Merchant.Location);

        Assert.Equal(request.Items.Sum(i => i.Amount), actualDto.Amount);
        Assert.Equal(request.Items.Sum(i => i.Amount), actualDto.Owing);
        Assert.Equal(0, actualDto.Paid);
    }
}