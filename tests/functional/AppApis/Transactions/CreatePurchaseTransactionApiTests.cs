using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.Types;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreatePurchaseTransactionApiTests(WebApplicationFactory<Program> factory) :
    CreateTransactionBaseApiTests(factory)
{
    private const TransactionEnums.TransactionKeys TRANSACTION_TYPE = TransactionEnums.TransactionKeys.PURCHASE;

    /// <summary>
    /// Tests that a Transaction can be created with an existing PayerPayee
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanCall_CreateTransaction_WithValidRequest_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccounts = (await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId.Equals(testUserId)))?.ToArray() ?? [];

        if (!existingAccounts.Any())
            Assert.Fail("Need to add accounts before running this test");

        foreach (var existingAccount in existingAccounts)
        {
            var transactionDate = GetRandomPastDate;

            var existingPayerPayees =
                (await PayerPayeeDocumentsRepository
                    .ListAsync(testUserId.ToString()))?
                .ValueOrDefault?
                .ToList() ??
                [];

            // Check for a random PayerPayee. If one can not be provided to you, then create a new random one.
            var existingPayerPayee = existingPayerPayees.Any() ?
                existingPayerPayees
                    .ToList()[new Random()
                        .Next(0, existingPayerPayees.Count - 1)] :
                default;

            var payerPayee = new PayerPayeeRequest
            {
                Id = existingPayerPayee?.Id ?? string.Empty,
                Name = existingPayerPayee?.Name ?? string.Empty,
                Description = existingPayerPayee?.Description ?? string.Empty,
                Location = existingPayerPayee?.Location ?? string.Empty
            };

            var transactionDescription = existingPayerPayee is null ?
                "Purchase Transaction Description - No PayerPayee" :
                $"Purchase Transaction Description with PayerPayee: " +
                $"{existingPayerPayee.Id} - {existingPayerPayee.Name}";

            // Arrange
            var createTransactionRequest = new CreatePurchaseTransactionRequest
            {
                UserId = testUserId.ToString(),
                AccountId = existingAccount.Id.ToString(),
                Items =
                [
                    new()
                    {
                        Amount = GetRandomAmount(100, 150),
                        CategoryId = ObjectId.GenerateNewId().ToString(),
                        Description = "Transaction Item 1 Description"
                    },

                    new()
                    {
                        Amount = GetRandomAmount(1, 250),
                        CategoryId = ObjectId.GenerateNewId().ToString(),
                        Description = "Transaction Item 2 Description"
                    }
                ],
                TransactionDate = transactionDate,
                Description = transactionDescription,
                PayerPayee = payerPayee
            };

            // Assert
            await AssertTransactionAsync(testUserId, existingAccount, createTransactionRequest, TRANSACTION_TYPE);
        }
    }

    /// <summary>
    /// Tests that a Purchase Transaction can be created with a Credit Account
    /// and that the Balance of the Account is INCREASED
    /// </summary>
    [Fact]
    public async Task CanCall_CreateTransaction_CreditAccount_WithValidRequest_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == testUserId &&
                a.AccountType == AccountEnums.AccountKeys.CREDITCARD);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = GetRandomPastDate;

        var existingPayerPayees =
            (await PayerPayeeDocumentsRepository
                .ListAsync(testUserId.ToString()))?
            .ValueOrDefault?
            .ToList() ??
            [];

        // Check for a random PayerPayee. If one can not be provided to you,
        // then create a new random one.
        var existingPayerPayee = existingPayerPayees.Any() ?
            existingPayerPayees
                .ToList()[new Random()
                    .Next(0, existingPayerPayees.Count - 1)] :
            default;

        var payerPayee = new PayerPayeeRequest
        {
            Id = existingPayerPayee?.Id ?? string.Empty,
            Name = existingPayerPayee?.Name ?? string.Empty,
            Description = existingPayerPayee?.Description ?? string.Empty,
            Location = existingPayerPayee?.Location ?? string.Empty
        };

        var transactionDescription = existingPayerPayee is null ?
            "Deposit Transaction Description - No PayerPayee" :
            $"Deposit Transaction Description with PayerPayee: " +
                $"{existingPayerPayee.Id} - {existingPayerPayee.Name}";

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionRequest
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),
            Items = new List<PurchaseTransactionItemRequest>
            {
                new()
                {
                    Amount = 250,
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
            Description = transactionDescription,
            PayerPayee = payerPayee
        };

        // Assert
        await AssertTransactionAsync(testUserId, existingAccount, createTransactionRequest, TRANSACTION_TYPE);
    }

    /// <summary>
    /// Tests that a Purchase Transaction can be created with a Debit Account
    /// and that the Balance of the Account is DECREASED.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanCall_CreateTransaction_DebitAccount_WithValidRequest_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == testUserId &&
                a.AccountType == AccountEnums.AccountKeys.CASH);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = GetRandomPastDate;

        var existingPayerPayees =
            (await PayerPayeeDocumentsRepository
                .ListAsync(testUserId.ToString()))?
            .ValueOrDefault?
            .ToList() ??
            [];

        // Check for a random PayerPayee. If one can not be provided to you, then create a new random one.
        var existingPayerPayee = existingPayerPayees.Any() ?
            existingPayerPayees
                .ToList()[new Random()
                    .Next(0, existingPayerPayees.Count - 1)] :
            default;

        var payerPayee = new PayerPayeeRequest
        {
            Id = existingPayerPayee?.Id ?? string.Empty,
            Name = existingPayerPayee?.Name ?? string.Empty,
            Description = existingPayerPayee?.Description ?? string.Empty,
            Location = existingPayerPayee?.Location ?? string.Empty
        };

        var transactionDescription = existingPayerPayee is null ?
            "Deposit Transaction Description - No PayerPayee" :
            $"Deposit Transaction Description with PayerPayee: " +
                $"{existingPayerPayee.Id} - {existingPayerPayee.Name}";

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionRequest
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),
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
            Description = transactionDescription,
            PayerPayee = payerPayee
        };

        // Assert
        await AssertTransactionAsync(testUserId, existingAccount, createTransactionRequest, TRANSACTION_TYPE);
    }


    /// <summary>
    /// Tests that a transaction can be created with a new PayerPayee
    /// </summary>
    [Fact]
    public async Task CanCall_CreateTransaction_WithNewPayerPayee_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a =>
            a.UserId == testUserId);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = GetRandomPastDate;

        var payerPayeeRandom = DateTime.Now.Ticks;

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionRequest
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),
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
            Description = $"Purchase Transaction Description - Expecting PayerPayee `{payerPayeeRandom}`",

            // New PayerPayee
            PayerPayee = new PayerPayeeRequest()
            {
                Id = string.Empty,
                Name = $"New PayerPayee {payerPayeeRandom}",
                Description = $"New PayerPayee {payerPayeeRandom} Description"
            }
        };

        // Assert
        await AssertTransactionAsync(testUserId, existingAccount, createTransactionRequest, TRANSACTION_TYPE);
    }

    /// <summary>
    /// Tests that a transaction can be created with a NO PayerPayee
    /// </summary>
    [Fact]
    public async Task CanCall_CreateTransaction_WithNoPayerPayee_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a =>
            a.UserId == testUserId);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = GetRandomPastDate;

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionRequest
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),
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
            Description = "Deposit Transaction Description - Not Expecting PayerPayee"
        };

        // Assert
        await AssertTransactionAsync(testUserId, existingAccount, createTransactionRequest, TRANSACTION_TYPE);
    }
}