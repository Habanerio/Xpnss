using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.Types;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreateDepositTransactionApiTests(WebApplicationFactory<Program> factory) :
    CreateTransactionBaseApiTests(factory)
{
    private const TransactionEnums.TransactionKeys TRANSACTION_TYPE =
        TransactionEnums.TransactionKeys.DEPOSIT;

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

            var payerPayee = new PayerPayeeApiRequest
            {
                Id = existingPayerPayee?.Id ?? string.Empty,
                Name = existingPayerPayee?.Name ?? string.Empty,
                Description = existingPayerPayee?.Description ?? string.Empty,
                Location = existingPayerPayee?.Location ?? string.Empty
            };

            var randomTransactionAmount = new Random().Next(100, 150);

            var transactionDescription = existingPayerPayee is null ?
                "Deposit Transaction Description - No PayerPayee" :
                $"Deposit Transaction Description with PayerPayee: " +
                $"{existingPayerPayee.Id.Value} - {existingPayerPayee.Name.Value}";

            // Arrange
            var createTransactionRequest = new CreateDepositTransactionApiRequest
            {
                UserId = testUserId.ToString(),
                AccountId = existingAccount.Id.ToString(),
                TotalAmount = randomTransactionAmount,
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

        var existingPayerPayeesResult =
            await PayerPayeeDocumentsRepository.ListAsync(testUserId.ToString());

        var existingPayerPayee = existingPayerPayeesResult.Value.Any() ?
                        existingPayerPayeesResult
                         .ValueOrDefault?
                         .ToList()[new Random().Next(0, existingPayerPayeesResult.Value.Count() - 1)] :
                     default;

        var transactionDescription = existingPayerPayee is null ?
            "Deposit Transaction Description - No PayerPayee" :
            $"Deposit Transaction Description with PayerPayee: {existingPayerPayee.Id} - {existingPayerPayee.Name}";

        var payerPayee = new PayerPayeeApiRequest
        {
            Id = existingPayerPayee?.Id ?? string.Empty,
            Name = existingPayerPayee?.Name ?? string.Empty,
            Description = existingPayerPayee?.Description ?? string.Empty,
            Location = existingPayerPayee?.Location ?? string.Empty
        };

        var randomTransactionAmount = new Random().Next(15, 250);

        // Arrange
        var createTransactionRequest = new CreateDepositTransactionApiRequest
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),
            TotalAmount = randomTransactionAmount,
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

        var existingPayerPayeesResult =
            await PayerPayeeDocumentsRepository.ListAsync(testUserId.ToString());

        var existingPayerPayee = existingPayerPayeesResult
                                     .ValueOrDefault?
                                     .ToList()[new Random().Next(0, existingPayerPayeesResult.Value.Count() - 1)] ??
                                 default;

        var transactionDescription = existingPayerPayee is null ?
            "Deposit Transaction Description - No PayerPayee" :
            $"Deposit Transaction Description with PayerPayee: {existingPayerPayee.Id} - {existingPayerPayee.Name}";

        var payerPayee = new PayerPayeeApiRequest
        {
            Id = existingPayerPayee?.Id ?? string.Empty,
            Name = existingPayerPayee?.Name ?? string.Empty,
            Description = existingPayerPayee?.Description ?? string.Empty,
            Location = existingPayerPayee?.Location ?? string.Empty
        };

        var randomTransactionAmount = new Random().Next(25, 500);

        // Arrange
        var createTransactionRequest = new CreateDepositTransactionApiRequest
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),
            TotalAmount = randomTransactionAmount,
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

        var transactionDate = DateTime.Now.AddDays(-(new Random().Next(1, 365 * 2)));

        var payerPayeeRandom = DateTime.Now.Ticks;

        var randomTransactionAmount = new Random().Next(1, 1000);

        // Arrange
        var createTransactionRequest = new CreateDepositTransactionApiRequest
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),

            TotalAmount = randomTransactionAmount,
            TransactionDate = transactionDate,
            Description = $"Deposit Transaction Description - Expecting PayerPayee `{payerPayeeRandom}`",

            // New PayerPayee
            PayerPayee = new PayerPayeeApiRequest()
            {
                Id = string.Empty,
                Name = $"New PayerPayee {payerPayeeRandom}",
                Description = $"New PayerPayee {payerPayeeRandom} Description",
                Location = $"New PayerPayee {payerPayeeRandom} Location"
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

        var existingAccount = await AccountDocumentsRepository
            .FirstOrDefaultDocumentAsync(a => a.UserId == testUserId);

        if (existingAccount is null)
            Assert.Fail("Need to add accounts before running this test");

        var transactionDate = GetRandomPastDate;

        // Arrange
        var createTransactionRequest = new CreateDepositTransactionApiRequest
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),
            TotalAmount = 999,
            TransactionDate = transactionDate,
            Description = "Deposit Transaction Description - Not Expecting PayerPayee"
        };

        // Assert
        await AssertTransactionAsync(testUserId, existingAccount, createTransactionRequest, TRANSACTION_TYPE);
    }
}