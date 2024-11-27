using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.Types;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreatePurchaseTransactionApiTests(WebApplicationFactory<Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_CREATE_TRANSACTION = "/api/v1/users/{userId}/transactions/purchase";

    private const TransactionTypes.Keys TRANSACTION_TYPE = TransactionTypes.Keys.PURCHASE;

    /// <summary>
    /// Tests that a Transaction can be created with an existing PayerPayee
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanCall_CreateTransaction_WithValidRequest_ReturnsOk()
    {
        var account = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a =>
            a.UserId == USER_ID);

        if (account is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = RandomTransactionDate;

        var existingPayerPayeesResult =
            await PayerPayeeDocumentsRepository.ListAsync(USER_ID);

        var existingPayerPayee = existingPayerPayeesResult
            .ValueOrDefault?
            .ToList()[new Random().Next(0, existingPayerPayeesResult.Value.Count() - 1)] ??
            default;

        var payerPayee = new PayerPayeeRequest
        {
            Id = existingPayerPayee?.Id ?? string.Empty,
            Name = existingPayerPayee?.Name ?? string.Empty,
            Description = existingPayerPayee?.Description ?? string.Empty,
            Location = existingPayerPayee?.Location ?? string.Empty
        };

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionRequest
        {
            UserId = USER_ID,
            AccountId = account.Id.ToString(),
            Items =
            [
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
            ],
            TransactionDate = transactionDate,
            Description = "Purchase Transaction Description",
            PayerPayee = payerPayee
        };

        // Assert
        await AssertTransactionAsync(account, createTransactionRequest);
    }

    /// <summary>
    /// Tests that a Purchase Transaction can be created with a Credit Account
    /// and that the Balance of the Account is INCREASED
    /// </summary>
    [Fact]
    public async Task CanCall_CreateTransaction_CreditAccount_WithValidRequest_ReturnsOk()
    {
        var account = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == USER_ID &&
                (AccountTypes.CreditAccountTypes.Contains(a.AccountType)));

        if (account is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = RandomTransactionDate;

        var existingPayerPayeesResult =
            await PayerPayeeDocumentsRepository.ListAsync(USER_ID);

        var existingPayerPayee = existingPayerPayeesResult
         .ValueOrDefault?
            .ToList()[new Random().Next(0, existingPayerPayeesResult.Value.Count() - 1)] ??
                default;

        var payerPayee = new PayerPayeeRequest
        {
            Id = existingPayerPayee?.Id ?? string.Empty,
            Name = existingPayerPayee?.Name ?? string.Empty,
            Description = existingPayerPayee?.Description ?? string.Empty,
            Location = existingPayerPayee?.Location ?? string.Empty
        };

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionRequest
        {
            UserId = USER_ID,
            AccountId = account.Id.ToString(),
            Items = new List<PurchaseTransactionItemRequest>
            {
                new()
                {
                    Amount = 250,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 1 Description"
                },
                new()
                {
                    Amount = 20,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 2 Description"
                }
            },
            TransactionDate = transactionDate,
            Description = "Purchase Transaction Description",
            PayerPayee = payerPayee
        };

        // Assert
        await AssertTransactionAsync(account, createTransactionRequest);
    }

    /// <summary>
    /// Tests that a Purchase Transaction can be created with a Debit Account
    /// and that the Balance of the Account is DECREASED.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanCall_CreateTransaction_DebitAccount_WithValidRequest_ReturnsOk()
    {
        var account = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == USER_ID &&
                (AccountTypes.DebitAccountTypes.Contains(a.AccountType)));

        if (account is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = RandomTransactionDate;

        var existingPayerPayeesResult =
            await PayerPayeeDocumentsRepository.ListAsync(USER_ID);

        var existingPayerPayee = existingPayerPayeesResult
                                 .ValueOrDefault?
                                 .ToList()[new Random().Next(0, existingPayerPayeesResult.Value.Count() - 1)] ??
                                 default;

        var payerPayee = new PayerPayeeRequest
        {
            Id = existingPayerPayee?.Id ?? string.Empty,
            Name = existingPayerPayee?.Name ?? string.Empty,
            Description = existingPayerPayee?.Description ?? string.Empty,
            Location = existingPayerPayee?.Location ?? string.Empty
        };

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionRequest
        {
            UserId = USER_ID,
            AccountId = account.Id.ToString(),
            Items =
            [
                new()
                {
                    Amount = 10,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 1 Description"
                },

                new()
                {
                    Amount = 123,
                    CategoryId = ObjectId.GenerateNewId().ToString(),
                    Description = "Transaction Item 2 Description"
                }
            ],
            TransactionDate = transactionDate,
            Description = "Purchase Transaction Description",
            PayerPayee = payerPayee
        };

        // Assert
        await AssertTransactionAsync(account, createTransactionRequest);
    }


    /// <summary>
    /// Tests that a transaction can be created with a new PayerPayee
    /// </summary>
    [Fact]
    public async Task CanCall_CreateTransaction_WithNewPayerPayee_ReturnsOk()
    {
        var account = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a => a.UserId == USER_ID);

        if (account is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = RandomTransactionDate;

        var payerPayeeRandom = DateTime.Now.Ticks;

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionRequest
        {
            UserId = USER_ID,
            AccountId = account.Id.ToString(),
            Items = new List<PurchaseTransactionItemRequest>
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
            TransactionDate = transactionDate,
            Description = "Purchase Transaction Description",

            // New PayerPayee
            PayerPayee = new PayerPayeeRequest()
            {
                Id = string.Empty,
                Name = $"New PayerPayee {payerPayeeRandom}",
                Description = $"New PayerPayee {payerPayeeRandom} Description"
            }
        };

        // Assert
        await AssertTransactionAsync(account, createTransactionRequest);
    }

    /// <summary>
    /// Tests that a transaction can be created with a NO PayerPayee
    /// </summary>
    [Fact]
    public async Task CanCall_CreateTransaction_WithNoPayerPayee_ReturnsOk()
    {
        var account = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a => a.UserId == USER_ID);

        if (account is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = RandomTransactionDate;

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionRequest
        {
            UserId = USER_ID,
            AccountId = account.Id.ToString(),
            Items = new List<PurchaseTransactionItemRequest>
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
            TransactionDate = transactionDate,
            Description = "Purchase Transaction Description",
        };

        // Assert
        await AssertTransactionAsync(account, createTransactionRequest);
    }

    /// <summary>
    /// Helper for Asserting the tests
    /// </summary>
    /// <param name="existingAccountDoc"></param>
    /// <param name="createTransactionRequest"></param>
    /// <returns></returns>
    private async Task AssertTransactionAsync(AccountDocument? existingAccountDoc, CreatePurchaseTransactionRequest createTransactionRequest)
    {
        if (existingAccountDoc is null)
            Assert.Fail("Need to add accounts before running this test");

        var previousAccountBalance = existingAccountDoc.Balance;

        // Act
        var createTransactionResponse = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_TRANSACTION
                .Replace("{userId}", USER_ID),
            createTransactionRequest);

        createTransactionResponse.EnsureSuccessStatusCode();

        var transactionContent = await createTransactionResponse.Content.ReadAsStringAsync();
        var transactionApiResponse = JsonSerializer.Deserialize<ApiResponse<PurchaseTransactionDto>>(transactionContent, JsonSerializationOptions);

        // Assert
        Assert.NotNull(transactionApiResponse);
        Assert.True(transactionApiResponse.IsSuccess);

        var actualTransactionDto = Assert.IsAssignableFrom<PurchaseTransactionDto>(transactionApiResponse.Data);

        Assert.NotNull(actualTransactionDto);
        Assert.True(!actualTransactionDto.Id.Equals(ObjectId.Empty.ToString()));
        Assert.Equal(createTransactionRequest.UserId, actualTransactionDto.UserId);
        Assert.Equal(createTransactionRequest.AccountId, actualTransactionDto.AccountId);
        Assert.Equal(createTransactionRequest.TransactionDate.Date, actualTransactionDto.TransactionDate.Date);
        Assert.Equal(TRANSACTION_TYPE.ToString(), actualTransactionDto.TransactionType);

        Assert.Equal(createTransactionRequest.Items.Count, actualTransactionDto.Items.Count);

        // If the request has a PayerPayee Id OR a Name, then it must return an Id (whether existing or new)
        if (!string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Id) || !string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Name))
        {
            Assert.NotNull(actualTransactionDto.PayerPayeeId);

            // If the request has a PayerPayee Id, then it must match the response
            if (!string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Id))
                Assert.Equal(createTransactionRequest.PayerPayee.Id, actualTransactionDto.PayerPayeeId);

            if (!string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Name))
            {
                Assert.NotNull(actualTransactionDto.PayerPayee);
                Assert.Equal(createTransactionRequest.PayerPayee.Name, actualTransactionDto.PayerPayee.Name);
                Assert.Equal(createTransactionRequest.PayerPayee.Description, actualTransactionDto.PayerPayee.Description);
                Assert.Equal(createTransactionRequest.PayerPayee.Location, actualTransactionDto.PayerPayee.Location);
            }
        }

        // New transaction, Total Owing should be the same as Total Amount
        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.TotalAmount);
        Assert.Equal(createTransactionRequest.Items.Sum(i => i.Amount), actualTransactionDto.TotalOwing);

        // New transaction, there should be no payments
        Assert.Equal(0, actualTransactionDto.TotalPaid);

        // Check that the Account Doc's Balance has been updated
        var updatedAccountDoc = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a => a.Id == existingAccountDoc.Id && a.UserId == USER_ID);
        Assert.NotNull(updatedAccountDoc);


        if (TransactionTypes.DoesBalanceIncrease(existingAccountDoc.AccountType, TRANSACTION_TYPE))
            Assert.Equal(previousAccountBalance + createTransactionRequest.TotalAmount, updatedAccountDoc.Balance);
        else
            Assert.Equal(previousAccountBalance - createTransactionRequest.TotalAmount, updatedAccountDoc.Balance);

        //Assert.NotEmpty(actualAccount.MonthlyTotals);

        //Assert.NotNull(actualAccount.MonthlyTotals.Find(t =>
        //    t.Year == transactionDate.Year && t.Month == transactionDate.Month));
    }
}