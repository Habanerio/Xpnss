using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.Merchants.DTOs;
using Habanerio.Xpnss.Application.Transactions.Commands;
using Habanerio.Xpnss.Application.Transactions.DTOs;
using Habanerio.Xpnss.Domain.Transactions;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreateTransactionApiTests(WebApplicationFactory<Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_CREATE_TRANSACTION = "/api/v1/users/{userId}/transactions";

    /// <summary>
    /// Tests that a transaction can be created with an existing merchant
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanCall_CreateTransaction_WithValidRequest_ReturnsOk()
    {
        var account = await AccountDocumentsRepository.FirstOrDefaultAsync(a => a.UserId == USER_ID);
        var previousAccountBalance = account.Balance;

        // Arrange
        var createTransactionRequest = new CreateTransactionCommand(
            USER_ID,
            account.Id.ToString(),
            new List<TransactionItemDto>
            {
                new TransactionItemDto()
                {
                    Amount = 100,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 1 Description"
                },
                new TransactionItemDto()
                {
                    Amount = 200,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 2 Description"
                }
            },
            DateTime.Now,
            TransactionTypes.Keys.PURCHASE.ToString(),
            "Transaction Description",
            new MerchantDto(
                ObjectId.GenerateNewId().ToString(),
                USER_ID,
                "Merchant Name",
                "Merchant Location"
            ));


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

        Assert.Equal(createTransactionRequest.Items.Count(), actualTransactionDto.Items.Count);

        Assert.NotNull(actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant.Id, actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant.Name, actualTransactionDto.MerchantName);
        Assert.Equal(createTransactionRequest.Merchant.Location, actualTransactionDto.MerchantLocation);

        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Amount);
        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Owing);
        Assert.Equal(0, actualTransactionDto.Paid);

        // Check that the Account's balance has been updated
        var actualAccount = await AccountDocumentsRepository.FirstOrDefaultAsync(a => a.Id == account.Id && a.UserId == USER_ID);
        Assert.NotNull(actualAccount);

        if (TransactionTypes.IsCredit(createTransactionRequest.TransactionType))
            Assert.Equal(previousAccountBalance + createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);
        else
            Assert.Equal(previousAccountBalance - createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);
    }

    /// <summary>
    /// Tests that a transaction can be created with a new merchant
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanCall_CreateTransaction_WithNewMerchant_ReturnsOk()
    {
        var account = await AccountDocumentsRepository.FirstOrDefaultAsync(a => a.UserId == USER_ID);
        var previousAccountBalance = account.Balance;

        // Arrange
        var createTransactionRequest = new CreateTransactionCommand(
            USER_ID,
            account.Id.ToString(),
            new List<TransactionItemDto>
            {
                new TransactionItemDto()
                {
                    Amount = 100,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 1 Description"
                },
                new TransactionItemDto()
                {
                    Amount = 200,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 2 Description"
                }
            },
            DateTime.Now,
            TransactionTypes.Keys.PURCHASE.ToString(),
            "Transaction Description",
            new MerchantDto(
                string.Empty,
                USER_ID,
                "New Merchant Name",
                "New Merchant Location"
            ));


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

        Assert.Equal(createTransactionRequest.Items.Count(), actualTransactionDto.Items.Count);

        Assert.NotNull(actualTransactionDto.MerchantId);
        Assert.NotEmpty(actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant.Name, actualTransactionDto.MerchantName);
        Assert.Equal(createTransactionRequest.Merchant.Location, actualTransactionDto.MerchantLocation);

        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Amount);
        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Owing);
        Assert.Equal(0, actualTransactionDto.Paid);

        // Check that the Account's balance has been updated
        var actualAccount = await AccountDocumentsRepository.FirstOrDefaultAsync(a => a.Id == account.Id && a.UserId == USER_ID);
        Assert.NotNull(actualAccount);

        if (TransactionTypes.IsCredit(createTransactionRequest.TransactionType))
            Assert.Equal(previousAccountBalance + createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);
        else
            Assert.Equal(previousAccountBalance - createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);
    }
}