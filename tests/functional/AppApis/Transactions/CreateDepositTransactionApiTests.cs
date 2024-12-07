using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.Types;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreateDepositTransactionApiTests(WebApplicationFactory<Program> factory) :
    CreateTransactionBaseApiTests(factory)
{
    private const TransactionEnums.TransactionKeys TRANSACTION_TYPE = TransactionEnums.TransactionKeys.DEPOSIT;

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

            var randomTransactionAmount = new Random().Next(100, 150);

            var transactionDescription = existingPayerPayee is null ?
                "Deposit Transaction Description - No PayerPayee" :
                $"Deposit Transaction Description with PayerPayee: " +
                $"{existingPayerPayee.Id} - {existingPayerPayee.Name}";

            // Arrange
            var createTransactionRequest = new CreateDepositTransactionRequest
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

        var payerPayee = new PayerPayeeRequest
        {
            Id = existingPayerPayee?.Id ?? string.Empty,
            Name = existingPayerPayee?.Name ?? string.Empty,
            Description = existingPayerPayee?.Description ?? string.Empty,
            Location = existingPayerPayee?.Location ?? string.Empty
        };

        var randomTransactionAmount = new Random().Next(15, 250);

        // Arrange
        var createTransactionRequest = new CreateDepositTransactionRequest
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

        var payerPayee = new PayerPayeeRequest
        {
            Id = existingPayerPayee?.Id ?? string.Empty,
            Name = existingPayerPayee?.Name ?? string.Empty,
            Description = existingPayerPayee?.Description ?? string.Empty,
            Location = existingPayerPayee?.Location ?? string.Empty
        };

        var randomTransactionAmount = new Random().Next(25, 500);

        // Arrange
        var createTransactionRequest = new CreateDepositTransactionRequest
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
        var createTransactionRequest = new CreateDepositTransactionRequest
        {
            UserId = testUserId.ToString(),
            AccountId = existingAccount.Id.ToString(),

            TotalAmount = randomTransactionAmount,
            TransactionDate = transactionDate,
            Description = $"Deposit Transaction Description - Expecting PayerPayee `{payerPayeeRandom}`",

            // New PayerPayee
            PayerPayee = new PayerPayeeRequest()
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
        var createTransactionRequest = new CreateDepositTransactionRequest
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

    ///// <summary>
    ///// Helper for Asserting the tests
    ///// </summary>
    ///// <param name="existingAccountDoc"></param>
    ///// <param name="createTransactionRequest"></param>
    ///// <returns></returns>
    //private async Task AssertTransactionAsync(
    //    AccountDocument? existingAccountDoc,
    //    CreateDepositTransactionRequest createTransactionRequest)
    //{
    //    var USER_ID = await GetTestUserObjectIdAsync();

    //    if (existingAccountDoc is null)
    //        Assert.Fail("Need to add accounts before running this test");

    //    var previousAccountBalance = existingAccountDoc.Balance;

    //    // Act
    //    var createTransactionResponse = await HttpClient.PostAsJsonAsync(
    //        ENDPOINTS_TRANSACTIONS_CREATE_TRANSACTION
    //            .Replace("{userId}", USER_ID.ToString()),
    //        createTransactionRequest);

    //    //createTransactionResponse.EnsureSuccessStatusCode();

    //    var transactionContent = await createTransactionResponse.Content.ReadAsStringAsync();

    //    if (!createTransactionResponse.IsSuccessStatusCode)
    //        Assert.Fail(transactionContent);

    //    var transactionApiResponse = JsonSerializer.Deserialize<ApiResponse<DepositTransactionDto>>(
    //        transactionContent, JsonSerializationOptions);

    //    // Assert
    //    Assert.NotNull(transactionApiResponse);
    //    Assert.True(transactionApiResponse.IsSuccess);

    //    var actualTransactionDto = Assert.IsType<DepositTransactionDto>(transactionApiResponse.Data);

    //    Assert.NotNull(actualTransactionDto);
    //    Assert.True(!actualTransactionDto.Id.Equals(ObjectId.Empty.ToString()));
    //    Assert.Equal(createTransactionRequest.UserId, actualTransactionDto.UserId);
    //    Assert.Equal(createTransactionRequest.ExtAcctId, actualTransactionDto.ExtAcctId);
    //    Assert.Equal(createTransactionRequest.TransactionDate.Date, actualTransactionDto.TransactionDate.Date);
    //    Assert.Equal(TRANSACTION_TYPE.ToString(), actualTransactionDto.TransactionType);


    //    // If the request has a PayerPayee Id OR a Name, then it must return an Id (whether existing or new)
    //    if (!string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Id) ||
    //        !string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Name))
    //    {
    //        Assert.NotNull(actualTransactionDto.PayerPayeeId);

    //        // If the request has a PayerPayee Id, then it must match the response
    //        if (!string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Id))
    //            Assert.Equal(createTransactionRequest.PayerPayee.Id, actualTransactionDto.PayerPayeeId);

    //        if (!string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Name))
    //        {
    //            Assert.NotNull(actualTransactionDto.PayerPayee);
    //            Assert.Equal(createTransactionRequest.PayerPayee.Name, actualTransactionDto.PayerPayee.Name);
    //            Assert.Equal(createTransactionRequest.PayerPayee.Description, actualTransactionDto.PayerPayee.Description);
    //            Assert.Equal(createTransactionRequest.PayerPayee.Location, actualTransactionDto.PayerPayee.Location);
    //        }
    //    }

    //    // Check that the Account Doc's Balance has been updated
    //    var updatedAccountDoc = await AccountDocumentsRepository
    //        .FirstOrDefaultDocumentAsync(a =>
    //            a.Id == existingAccountDoc.Id && a.UserId == USER_ID);

    //    Assert.NotNull(updatedAccountDoc);


    //    //// DEPOSIT + CREDIT CARD = Balance (owed) Decreases (GOOD Thing)
    //    //if (TransactionEnums.IsCreditTransaction(TRANSACTION_TYPE) && AccountEnums.CurrencyKeys.IsCreditAccount(existingAccountDoc.AccountType))
    //    //    Assert.Equal(previousAccountBalance - createTransactionRequest.TotalAmount, updatedAccountDoc.Balance);

    //    //// PURCHASE + CHECKING = Balance Decreases (BAD Thing)
    //    //if (!TransactionEnums.IsCreditTransaction(TRANSACTION_TYPE) && !AccountEnums.CurrencyKeys.IsCreditAccount(existingAccountDoc.AccountType))
    //    //    Assert.Equal(previousAccountBalance - createTransactionRequest.TotalAmount, updatedAccountDoc.Balance);


    //    //// PURCHASE + CREDIT CARD = Balance (owed) Increases (BAD Thing)
    //    //if (!TransactionEnums.IsCreditTransaction(TRANSACTION_TYPE) && AccountEnums.CurrencyKeys.IsCreditAccount(existingAccountDoc.AccountType))
    //    //    Assert.Equal(previousAccountBalance + createTransactionRequest.TotalAmount, updatedAccountDoc.Balance);

    //    //// DEPOSIT + CHECKING = Balance Increases (GOOD Thing)
    //    //if (TransactionEnums.IsCreditTransaction(TRANSACTION_TYPE) && !AccountEnums.CurrencyKeys.IsCreditAccount(existingAccountDoc.AccountType))
    //    //    Assert.Equal(previousAccountBalance + createTransactionRequest.TotalAmount, updatedAccountDoc.Balance);


    //    var doesBalanceIncrease = TransactionEnums.DoesBalanceIncrease(existingAccountDoc.AccountType, TRANSACTION_TYPE);

    //    if (doesBalanceIncrease)
    //        Assert.Equal(previousAccountBalance + createTransactionRequest.TotalAmount, updatedAccountDoc.Balance);
    //    else
    //        Assert.Equal(previousAccountBalance - createTransactionRequest.TotalAmount, updatedAccountDoc.Balance);


    //    //Assert.NotEmpty(actualAccount.MonthlyTotals);

    //    //Assert.NotNull(actualAccount.MonthlyTotals.Find(t =>
    //    //    t.Year == transactionDate.Year && t.Month == transactionDate.Month));
    //}
}