using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Transactions.Application.Commands;
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
        var account = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a => a.UserId == USER_ID);

        if (account is null)
            return;

        var previousAccountBalance = account.Balance;

        var transactionDate = DateTime.Now;
        var transactionType = TransactionTypes.Keys.PURCHASE;
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
            transactionDate,
            transactionType.ToString(),
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
        var transactionApiResponse = JsonSerializer.Deserialize<ApiResponse<TransactionDto>>(transactionContent, JsonSerializationOptions);

        // Assert
        Assert.NotNull(transactionApiResponse);
        Assert.True(transactionApiResponse.IsSuccess);

        var actualTransactionDto = Assert.IsType<TransactionDto>(transactionApiResponse.Data);
        Assert.True(!actualTransactionDto.Id.Equals(ObjectId.Empty.ToString()));
        Assert.Equal(createTransactionRequest.UserId, actualTransactionDto.UserId);
        Assert.Equal(createTransactionRequest.AccountId, actualTransactionDto.AccountId);
        Assert.Equal(transactionDate.Date, actualTransactionDto.TransactionDate.Date);
        Assert.Equal(createTransactionRequest.TransactionType, actualTransactionDto.TransactionType);

        Assert.Equal(createTransactionRequest.Items.Count(), actualTransactionDto.Items.Count);

        Assert.NotNull(actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant.Id, actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant.Name, actualTransactionDto.MerchantName);
        Assert.Equal(createTransactionRequest.Merchant.Location, actualTransactionDto.MerchantLocation);

        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.TotalAmount);
        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Owing);
        Assert.Equal(0, actualTransactionDto.Paid);

        // Check that the Account's balance has been updated
        var actualAccount = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a => a.Id == account.Id && a.UserId == USER_ID);
        Assert.NotNull(actualAccount);

        if (TransactionTypes.IsCreditTransaction(actualAccount.AccountType, transactionType))
            Assert.Equal(previousAccountBalance + createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);
        else
            Assert.Equal(previousAccountBalance - createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);

        //Assert.NotEmpty(actualAccount.MonthlyTotals);

        //Assert.NotNull(actualAccount.MonthlyTotals.Find(t =>
        //    t.Year == transactionDate.Year && t.Month == transactionDate.Month));
    }

    [Fact]
    public async Task CanCall_CreateTransaction_CreditAccount_WithValidRequest_ReturnsOk()
    {
        var account = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == USER_ID &&
                (AccountTypes.CreditAccountTypes.Contains(a.AccountType)));

        if (account is null)
            return;

        var previousAccountBalance = account.Balance;

        var transactionDate = DateTime.Now.AddDays(-(new Random().Next(1, 365 * 2)));

        // Arrange
        var createTransactionRequest = new CreateTransactionCommand(
            USER_ID,
            account.Id.ToString(),
            new List<TransactionItemDto>
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
                }
            },
            transactionDate,
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
        var transactionApiResponse = JsonSerializer.Deserialize<ApiResponse<TransactionDto>>(transactionContent, JsonSerializationOptions);

        // Assert
        Assert.NotNull(transactionApiResponse);
        Assert.True(transactionApiResponse.IsSuccess);

        var actualTransactionDto = Assert.IsType<TransactionDto>(transactionApiResponse.Data);
        Assert.True(!actualTransactionDto.Id.Equals(ObjectId.Empty.ToString()));
        Assert.Equal(createTransactionRequest.UserId, actualTransactionDto.UserId);
        Assert.Equal(createTransactionRequest.AccountId, actualTransactionDto.AccountId);
        Assert.Equal(transactionDate.Date, actualTransactionDto.TransactionDate.Date);
        Assert.Equal(createTransactionRequest.TransactionType, actualTransactionDto.TransactionType);

        Assert.Equal(createTransactionRequest.Items.Count(), actualTransactionDto.Items.Count);

        Assert.NotNull(actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant.Id, actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant.Name, actualTransactionDto.MerchantName);
        Assert.Equal(createTransactionRequest.Merchant.Location, actualTransactionDto.MerchantLocation);

        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.TotalAmount);
        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Owing);
        Assert.Equal(0, actualTransactionDto.Paid);

        // Check that the Account's balance has been updated
        var actualAccount = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a => a.Id == account.Id && a.UserId == USER_ID);
        Assert.NotNull(actualAccount);

        if (TransactionTypes.IsCreditTransaction(createTransactionRequest.TransactionType))
            Assert.Equal(previousAccountBalance - createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);
        else
            Assert.Equal(previousAccountBalance + createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);

        //Assert.NotEmpty(actualAccount.MonthlyTotals);

        //Assert.NotNull(actualAccount.MonthlyTotals.Find(t =>
        //    t.Year == transactionDate.Year && t.Month == transactionDate.Month));
    }

    [Fact]
    public async Task CanCall_CreateTransaction_DebitAccount_WithValidRequest_ReturnsOk()
    {
        var account = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == USER_ID &&
                (AccountTypes.DebitAccountTypes.Contains(a.AccountType)));

        if (account is null)
            return;

        var previousAccountBalance = account.Balance;

        var transactionDate = DateTime.Now.AddDays(-(new Random().Next(1, 365 * 2)));

        // Arrange
        var createTransactionRequest = new CreateTransactionCommand(
            USER_ID,
            account.Id.ToString(),
            new List<TransactionItemDto>
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
                }
            },
            transactionDate,
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
        var transactionApiResponse = JsonSerializer.Deserialize<ApiResponse<TransactionDto>>(transactionContent, JsonSerializationOptions);

        // Assert
        Assert.NotNull(transactionApiResponse);
        Assert.True(transactionApiResponse.IsSuccess);

        var actualTransactionDto = Assert.IsType<TransactionDto>(transactionApiResponse.Data);
        Assert.True(!actualTransactionDto.Id.Equals(ObjectId.Empty.ToString()));
        Assert.Equal(createTransactionRequest.UserId, actualTransactionDto.UserId);
        Assert.Equal(createTransactionRequest.AccountId, actualTransactionDto.AccountId);
        Assert.Equal(transactionDate.Date, actualTransactionDto.TransactionDate.Date);
        Assert.Equal(createTransactionRequest.TransactionType, actualTransactionDto.TransactionType);

        Assert.Equal(createTransactionRequest.Items.Count(), actualTransactionDto.Items.Count);

        Assert.NotNull(actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant.Id, actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant.Name, actualTransactionDto.MerchantName);
        Assert.Equal(createTransactionRequest.Merchant.Location, actualTransactionDto.MerchantLocation);

        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.TotalAmount);
        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Owing);
        Assert.Equal(0, actualTransactionDto.Paid);

        // Check that the Account's balance has been updated
        var actualAccount = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a => a.Id == account.Id && a.UserId == USER_ID);
        Assert.NotNull(actualAccount);

        if (TransactionTypes.IsCreditTransaction(createTransactionRequest.TransactionType))
            Assert.Equal(previousAccountBalance + createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);
        else
            Assert.Equal(previousAccountBalance - createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);

        //Assert.NotEmpty(actualAccount.MonthlyTotals);

        //Assert.NotNull(actualAccount.MonthlyTotals.Find(t =>
        //    t.Year == transactionDate.Year && t.Month == transactionDate.Month));
    }


    /// <summary>
    /// Tests that a transaction can be created with a new merchant
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanCall_CreateTransaction_WithNewMerchant_ReturnsOk()
    {
        var account = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a => a.UserId == USER_ID);
        var previousAccountBalance = account.Balance;

        var transactionDate = DateTime.Now.AddDays(-(new Random().Next(1, 365 * 2)));

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
            transactionDate,
            TransactionTypes.Keys.PURCHASE.ToString(),
            "Transaction Description",
            new MerchantDto(
                string.Empty,
                USER_ID,
                "NewId Merchant Name",
                "NewId Merchant Location"
            ));


        // Act
        var createTransactionResponse = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_TRANSACTION
                .Replace("{userId}", USER_ID),
            createTransactionRequest);

        createTransactionResponse.EnsureSuccessStatusCode();

        var transactionContent = await createTransactionResponse.Content.ReadAsStringAsync();
        var transactionApiResponse = JsonSerializer.Deserialize<ApiResponse<TransactionDto>>(transactionContent, JsonSerializationOptions);

        // Assert
        Assert.NotNull(transactionApiResponse);
        Assert.True(transactionApiResponse.IsSuccess);

        var actualTransactionDto = Assert.IsType<TransactionDto>(transactionApiResponse.Data);
        Assert.True(!actualTransactionDto.Id.Equals(ObjectId.Empty.ToString()));
        Assert.Equal(createTransactionRequest.UserId, actualTransactionDto.UserId);
        Assert.Equal(createTransactionRequest.AccountId, actualTransactionDto.AccountId);
        Assert.Equal(transactionDate.Date, actualTransactionDto.TransactionDate.Date);
        Assert.Equal(createTransactionRequest.TransactionType, actualTransactionDto.TransactionType);

        Assert.Equal(createTransactionRequest.Items.Count(), actualTransactionDto.Items.Count);

        Assert.NotNull(actualTransactionDto.MerchantId);
        Assert.NotEmpty(actualTransactionDto.MerchantId);
        Assert.Equal(createTransactionRequest.Merchant?.Name, actualTransactionDto.MerchantName);
        Assert.Equal(createTransactionRequest.Merchant?.Location, actualTransactionDto.MerchantLocation);

        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.TotalAmount);
        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.Owing);
        Assert.Equal(0, actualTransactionDto.Paid);

        // Check that the Account's balance has been updated
        var actualAccount = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a => a.Id == account.Id && a.UserId == USER_ID);
        Assert.NotNull(actualAccount);

        if (TransactionTypes.IsCreditTransaction(createTransactionRequest.TransactionType))
            Assert.Equal(previousAccountBalance + createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);
        else
            Assert.Equal(previousAccountBalance - createTransactionRequest.Items.Sum(t => t.Amount), actualAccount.Balance);

        //Assert.NotEmpty(actualAccount.MonthlyTotals);

        //Assert.NotNull(actualAccount.MonthlyTotals.Find(t =>
        //    t.Year == transactionDate.Year && t.Month == transactionDate.Month));
    }
}