using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.Types;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreatePurchaseTransactionApiTests(WebApplicationFactory<Program> factory) :
    CreateTransactionBaseApiTests(factory)
{
    private const TransactionEnums.TransactionKeys TRANSACTION_TYPE = TransactionEnums.TransactionKeys.PURCHASE;

    /// <summary>
    /// Tests that a Purchase Transaction can be created with a Credit Account
    /// and that the Balance of the Account is INCREASED
    /// </summary>
    [Fact]
    public async Task CanCall_CreatePurchaseTransaction_CreditAccount_WithValidRequest_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == testUserId &&
                a.AccountType == AccountEnums.AccountKeys.CREDITCARD);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        await CanCall_CreatePurchaseTransaction_WithValidRequest_ReturnsOk(
            testUserId,
            existingAccount,
            true);
    }

    /// <summary>
    /// Tests that a Purchase Transaction can be created with a Debit Account
    /// and that the Balance of the Account is DECREASED.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanCall_CreatePurchaseTransaction_DebitAccount_WithValidRequest_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == testUserId &&
                a.AccountType == AccountEnums.AccountKeys.CASH);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        await CanCall_CreatePurchaseTransaction_WithValidRequest_ReturnsOk(
            testUserId,
            existingAccount,
            true);
    }

    /// <summary>
    /// Tests that a transaction can be created with a new PayerPayee
    /// </summary>
    [Fact]
    public async Task CanCall_CreatePurchaseTransaction_WithNewPayerPayee_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
            a.UserId == testUserId);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        await CanCall_CreatePurchaseTransaction_WithValidRequest_ReturnsOk(
            testUserId,
            existingAccount,
            false);
    }

    /// <summary>
    /// Tests that a transaction can be created with a NO PayerPayee
    /// </summary>
    [Fact]
    public async Task CanCall_CreatePurchaseTransaction_WithNoPayerPayee_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
            a.UserId == testUserId);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        await CanCall_CreatePurchaseTransaction_WithValidRequest_ReturnsOk(
            testUserId,
            existingAccount,
            null);
    }


    private async Task CanCall_CreatePurchaseTransaction_WithValidRequest_ReturnsOk(
        ObjectId testUserId,
        AccountDocument existingAccount,
        bool? useExistingPayerPayee)
    {
        var random = new Random();

        PayerPayeeRequest? randomPayerPayeeRequest = null;

        if (useExistingPayerPayee is null)
        {
            randomPayerPayeeRequest = null;
        }
        else if (useExistingPayerPayee == true)
        {
            randomPayerPayeeRequest = new PayerPayeeRequest
            { Name = GetRandomMerchant() };
        }
        else
        {
            randomPayerPayeeRequest = new PayerPayeeRequest
            { Name = $"Random PayerPayee - {DateTime.Now.Ticks}" };
        }

        var transactionDescription = randomPayerPayeeRequest is null ||
            string.IsNullOrWhiteSpace(randomPayerPayeeRequest.Name) ?
            "Purchase Transaction Description - No PayerPayee" :
            $"Purchase Transaction Description with PayerPayee: " +
            $"{randomPayerPayeeRequest.Id} - {randomPayerPayeeRequest.Name}";


        var transactionDate = GetRandomPastDate;

        var category1 = await GetHomeExpensesCategoryAsync();
        var subCategory1 = category1.SubCategories[random.Next(0, category1.SubCategories.Count - 1)];

        var category2 = await GetPersonalExpensesCategoryAsync();
        var subCategory2 = category2.SubCategories[random.Next(0, category2.SubCategories.Count - 1)];

        // Arrange
        var createTransactionRequest = new CreatePurchaseTransactionApiRequest(
            testUserId.ToString(),
            existingAccount.Id.ToString(),
            randomPayerPayeeRequest ?? new PayerPayeeRequest(),
            transactionDescription,
            transactionDate,
            new List<TransactionApiRequestItem>
            {
                new()
                {
                    Amount = 35,
                    CategoryId = category1.Id.ToString(),
                    SubCategoryId = subCategory1.Id.ToString(),
                    Description = "Transaction Item 1 Description"
                },

                new()
                {
                    Amount = 65,
                    CategoryId = category1.Id.ToString(),
                    SubCategoryId = subCategory2.Id.ToString(),
                    Description = "Transaction Item 2 Description"
                }
            },
            new List<string> { "tag1", "tag3", "tag 7" },
            "extTransactionId");

        // Assert
        await AssertTransactionAsync(testUserId, existingAccount, createTransactionRequest, TRANSACTION_TYPE);
    }
}