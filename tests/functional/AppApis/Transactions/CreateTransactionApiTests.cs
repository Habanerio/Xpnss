using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Transactions.Common;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreateTransactionApiTests : BaseFunctionalApisTests,
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_CREATE_TRANSACTION = "/api/v1/users/{userId}/transactions";
    private const string ENDPOINTS_GET_ACCOUNT = "/api/v1/users/{userId}/accounts/{accountId}";
    private const string ENDPOINTS_GET_ACCOUNTS = "/api/v1/users/{userId}/accounts";

    public CreateTransactionApiTests(WebApplicationFactory<Program> factory) :
        base(factory)
    { }

    [Fact]
    public async Task CanCall_CreateTransaction_WithValidRequest_ReturnsOk()
    {
        var accounts = await GetAccountDocsAsync();
        var account = accounts.FirstOrDefault(a => a.AccountType == AccountTypes.Cash);

        // Arrange
        var createTransactionRequest = new CreateTransactionEndpoint.CreateTransactionRequest
        {
            UserId = USER_ID,
            AccountId = account.Id.ToString(),
            TransactionDate = DateTime.Now,
            TransactionType = TransactionTypes.Keys.PURCHASE.ToString(),
            Items = new List<CreateTransactionEndpoint.CreateTransactionRequest.TransactionItem>
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
            TransactionMerchant = new CreateTransactionEndpoint.CreateTransactionRequest.Merchant
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Merchant Name",
                Location = "Merchant Location"
            }
        };

        // Act
        var createTransactionResponse = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_TRANSACTION
                .Replace("{userId}", USER_ID),
            createTransactionRequest);

        createTransactionResponse.EnsureSuccessStatusCode();

        var transactionContent = await createTransactionResponse.Content.ReadAsStringAsync();
        var transactionApiResponse = JsonSerializer.Deserialize<ApiResponse<TransactionDto>>(transactionContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(transactionApiResponse);
        Assert.True(transactionApiResponse.IsSuccess);

        var actualTransactionDto = Assert.IsType<TransactionDto>(transactionApiResponse.Data);
        Assert.True(!actualTransactionDto.Id.Equals(ObjectId.Empty.ToString()));
        Assert.Equal(createTransactionRequest.UserId, actualTransactionDto.UserId);
        Assert.Equal(createTransactionRequest.AccountId, actualTransactionDto.AccountId);
        Assert.Equal(DateTime.Now.Date, actualTransactionDto.TransactionDate.Date);
        Assert.Equal(createTransactionRequest.TransactionType, actualTransactionDto.TransactionType);

        Assert.Equal(createTransactionRequest.Items.Count, actualTransactionDto.Items.Count);

        Assert.NotNull(actualTransactionDto.Merchant);
        Assert.Equal(createTransactionRequest.TransactionMerchant.Id, actualTransactionDto.Merchant.Id);
        Assert.Equal(createTransactionRequest.TransactionMerchant.Name, actualTransactionDto.Merchant.Name);
        Assert.Equal(createTransactionRequest.TransactionMerchant.Location, actualTransactionDto.Merchant.Location);

        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Amount);
        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Owing);
        Assert.Equal(0, actualTransactionDto.Paid);
    }

    [Fact]
    public async Task CanCall_CreateTransaction_WithValidRequest_Validate_Account()
    {
        var accounts = await GetAccountDocsAsync();
        var account = accounts.FirstOrDefault(a => a.AccountType == AccountTypes.Cash);

        // Arrange
        var createTransactionRequest = new CreateTransactionEndpoint.CreateTransactionRequest
        {
            UserId = USER_ID,
            AccountId = account.Id.ToString(),
            TransactionDate = DateTime.Now,
            TransactionType = TransactionTypes.Keys.PURCHASE.ToString(),
            Items = new List<CreateTransactionEndpoint.CreateTransactionRequest.TransactionItem>
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
            TransactionMerchant = new CreateTransactionEndpoint.CreateTransactionRequest.Merchant
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Merchant Name",
                Location = "Merchant Location"
            }
        };

        // Act
        var createTransactionResponse = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_TRANSACTION
                .Replace("{userId}", USER_ID),
            createTransactionRequest);

        createTransactionResponse.EnsureSuccessStatusCode();

        var getAccountResponse = await HttpClient.GetAsync(
            ENDPOINTS_GET_ACCOUNT
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", createTransactionRequest.AccountId));

        getAccountResponse.EnsureSuccessStatusCode();

        var accountContent = await getAccountResponse.Content.ReadAsStringAsync();
        var accountApiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(accountContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(accountApiResponse);
        Assert.True(accountApiResponse.IsSuccess);

        var actualAccountDto = Assert.IsType<AccountDto>(accountApiResponse.Data);
        Assert.True(actualAccountDto.Balance >= createTransactionRequest.Items.Sum(i => i.Amount));
        Assert.NotEmpty(actualAccountDto.MonthlyDepositTotals);
        Assert.NotEmpty(actualAccountDto.MonthlyWithdrawTotals);
    }
}