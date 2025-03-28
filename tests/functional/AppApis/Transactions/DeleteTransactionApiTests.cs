using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class DeleteTransactionApiTests(WebApplicationFactory<Program> factory) :
    CreateTransactionBaseApiTests(factory)
{
    [Fact]
    public async Task CanCall_DeleteTransaction_WithValidRequest_Returns_Ok()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingTransaction = await TransactionDocumentsRepository
            .FirstOrDefaultDocumentAsync(t =>
                t.UserId == testUserId &&
                !t.IsDeleted);

        if (existingTransaction is null)
            Assert.Fail("Need to add transactions before running this test");

        var originalMonthlyTotals =
            (await MonthlyTotalDocumentsRepository
            .FindDocumentsAsync(mt =>
                mt.UserId == testUserId &&
                mt.Year == existingTransaction.TransactionDate.Year &&
                mt.Month == existingTransaction.TransactionDate.Month)).ToList();

        var now = DateTime.UtcNow;

        var response = await XpnssApiClient.DeleteAsync(
            ENDPOINTS_TRANSACTIONS_DELETE_TRANSACTION
                .Replace("{userId}", testUserId.ToString())
                .Replace("{transactionId}", existingTransaction.Id.ToString()));

        Assert.NotNull(response);

        var content = await response.Content.ReadAsStringAsync();

        Assert.NotNull(content);

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);
        }

        var result = await response.Content.ReadFromJsonAsync<bool>();

        Assert.True(result);

        var deletedTransaction = await TransactionDocumentsRepository
            .FirstOrDefaultDocumentAsync(t =>
                t.UserId == testUserId &&
                t.Id == existingTransaction.Id);

        Assert.NotNull(deletedTransaction);
        Assert.True(deletedTransaction.IsDeleted);
        Assert.NotNull(deletedTransaction.DateDeleted);
        Assert.Equal(
            now,
            deletedTransaction.DateDeleted.Value,
            new TimeSpan(0, 0, 0, 10));

        var updatedMonthlyTotals =
            (await MonthlyTotalDocumentsRepository
            .FindDocumentsAsync(mt =>
                mt.UserId == testUserId &&
                mt.Year == existingTransaction.TransactionDate.Year &&
                mt.Month == existingTransaction.TransactionDate.Month)).ToList();

        var x = true;
    }

    [Fact]
    public async Task CannotCall_DeleteTransaction_TransactionAlreadyDeleted_Returns_BadRequest()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingTransaction = await TransactionDocumentsRepository
            .FirstOrDefaultDocumentAsync(t =>
                t.UserId == testUserId &&
                t.IsDeleted);

        if (existingTransaction is null)
            Assert.Fail("Need to add or delete transactions before running this test");

        var originalMonthlyTotals =
            (await MonthlyTotalDocumentsRepository
            .FindDocumentsAsync(mt =>
                mt.UserId == testUserId &&
                mt.Year == existingTransaction.TransactionDate.Year &&
                mt.Month == existingTransaction.TransactionDate.Month)).ToList();

        var now = DateTime.UtcNow;

        var response = await XpnssApiClient.DeleteAsync(
            ENDPOINTS_TRANSACTIONS_DELETE_TRANSACTION
                .Replace("{userId}", testUserId.ToString())
                .Replace("{transactionId}", existingTransaction.Id.ToString()));

        Assert.NotNull(response);

        var content = await response.Content.ReadAsStringAsync();

        Assert.NotNull(content);
        Assert.False(response.IsSuccessStatusCode);

        var error = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);

        Assert.NotNull(error);
        Assert.NotEmpty(error);

        Assert.True(error.Any(e => e.Equals("Transaction is already deleted!")));

        var updatedMonthlyTotals =
            (await MonthlyTotalDocumentsRepository
            .FindDocumentsAsync(mt =>
                mt.UserId == testUserId &&
                mt.Year == existingTransaction.TransactionDate.Year &&
                mt.Month == existingTransaction.TransactionDate.Month)).ToList();

        var x = true;
    }
}