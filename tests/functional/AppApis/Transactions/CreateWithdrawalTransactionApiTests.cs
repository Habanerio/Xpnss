using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.Types;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreateWithdrawalTransactionApiTests(WebApplicationFactory<Program> factory) :
    CreateTransactionBaseApiTests(factory)
{
    private const TransactionEnums.TransactionKeys TRANSACTION_TYPE =
        TransactionEnums.TransactionKeys.WITHDRAWAL;

    /// <summary>
    /// Tests that a Purchase Transaction can be created with a Credit Account
    /// and that the Balance of the Account is INCREASED
    /// </summary>
    [Fact]
    public async Task CanCall_CreateWithdrawalTransaction_CreditAccount_WithValidRequest_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == testUserId &&
                a.AccountType == AccountEnums.AccountKeys.CREDITCARD);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        await CanCall_CreateWithdrawalTransaction_WithValidRequest_ReturnsOk(
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
    public async Task CanCall_CreateWithdrawalTransaction_DebitAccount_WithValidRequest_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a =>
                a.UserId == testUserId &&
                a.AccountType == AccountEnums.AccountKeys.CASH);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        await CanCall_CreateWithdrawalTransaction_WithValidRequest_ReturnsOk(
            testUserId,
            existingAccount,
            true);
    }


    /// <summary>
    /// Tests that a transaction can be created with a new PayerPayee
    /// </summary>
    [Fact]
    public async Task CanCall_CreateWithdrawalTransaction_WithNewPayerPayee_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a =>
            a.UserId == testUserId);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        await CanCall_CreateWithdrawalTransaction_WithValidRequest_ReturnsOk(
            testUserId,
            existingAccount,
            false);
    }

    /// <summary>
    /// Tests that a transaction can be created with a NO PayerPayee
    /// </summary>
    [Fact]
    public async Task CanCall_CreateWithdrawalTransaction_WithNoPayerPayee_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a => a.UserId == testUserId);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        await CanCall_CreateWithdrawalTransaction_WithValidRequest_ReturnsOk(
            testUserId,
            existingAccount,
            null);
    }

    private async Task CanCall_CreateWithdrawalTransaction_WithValidRequest_ReturnsOk(
        ObjectId testUserId,
        AccountDocument existingAccount,
        bool? useExistingPayerPayee)
    {
        var random = new Random();

        // Withdrawals go from one account to the other
        var accounts = (await AccountDocumentsRepository
            .FindDocumentsAsync(a => a.UserId.Equals(testUserId))).ToList();

        var randomAccount = accounts[random.Next(0, accounts.Count - 1)];

        var randomPayerPayeeRequest = new PayerPayeeRequest();

        if (useExistingPayerPayee is null)
        {
            // Do nothing
        }
        else if (useExistingPayerPayee == true)
        {
            randomPayerPayeeRequest = new PayerPayeeRequest
            {
                Id = randomAccount.Id.ToString(),
                Name = randomAccount.Name
            };
        }
        else
        {
            randomPayerPayeeRequest = new PayerPayeeRequest
            {
                Id = randomAccount.Id.ToString(),
                Name = randomAccount.Name
            };
        }

        var transactionDescription = string.IsNullOrWhiteSpace(randomPayerPayeeRequest.Id) ?
            "Withdrawal Transaction Description - No WithdrawTo Account" :
            $"Withdrawal Transaction Description with WithdrawTo Account: " +
            $"{randomPayerPayeeRequest.Id} - {randomPayerPayeeRequest.Name}";

        var transactionDate = GetRandomPastDate;

        // Arrange
        var createTransactionRequest = new CreateWithdrawalTransactionRequest()
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),
            TotalAmount = 999,
            TransactionDate = transactionDate,
            Description = transactionDescription,
            PayerPayee = randomPayerPayeeRequest
        };

        // Assert
        await AssertTransactionAsync(testUserId, existingAccount, createTransactionRequest, TRANSACTION_TYPE);
    }
}