using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.Types;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CopyTransactionApiTests(WebApplicationFactory<Program> factory) :
    CreateTransactionBaseApiTests(factory)
{

    [Fact]
    public async Task CanCall_CopyTransaction_WithValidRequest_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingTransaction = await TransactionDocumentsRepository
            .FirstOrDefaultDocumentAsync(t =>
                t.UserId == testUserId);

        if (existingTransaction is null)
            Assert.Fail("Need to add transactions before running this test");

        var request = new CopyTransactionRequest[]
        {
            new CopyTransactionRequest(2019, 1, 1),
            new CopyTransactionRequest(2020, 1, 1),
            new CopyTransactionRequest(2021, 1, 1),
        };

        var response = await XpnssApiClient.PostAsJsonAsync(
            ENDPOINTS_TRANSACTIONS_COPY_TRANSACTION
                .Replace("{userId}", testUserId.ToString())
                .Replace("{transactionId}", existingTransaction.Id.ToString()),
            request);

        Assert.NotNull(response);

        var content = await response.Content.ReadAsStringAsync();

        Assert.NotNull(content);

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);
        }

        var transactionDtos =
            (await response.Content.ReadFromJsonAsync<IEnumerable<TransactionDto>>())?.ToList() ??
            [];

        Assert.NotNull(transactionDtos);
        Assert.NotEmpty(transactionDtos);

        // RefTransactionId must be the same as the existing transaction
        Assert.True(transactionDtos.All(t =>
            t.RefTransactionId.Equals(existingTransaction.Id.ToString())));

        Assert.True(transactionDtos.All(t =>
            t.UserId.Equals(existingTransaction.UserId.ToString())));

        Assert.True(transactionDtos.All(t =>
            t.AccountId.Equals(existingTransaction.AccountId.ToString())));

        Assert.True(transactionDtos.All(t =>
            t.Description.Equals(existingTransaction.Description)));

        Assert.True(transactionDtos.All(t =>
            t.ExtTransactionId.Equals(existingTransaction.ExtTransactionId)));

        Assert.True(transactionDtos.All(t =>
            t.IsCredit.Equals(existingTransaction.IsCredit)));

        Assert.True(transactionDtos.All(t =>
            t.PayerPayeeId.Equals(existingTransaction.PayerPayeeId?.ToString())));

        Assert.True(transactionDtos.All(t =>
            t.Tags.SequenceEqual(existingTransaction.Tags)));

        Assert.True(transactionDtos.All(t =>
            t.TotalAmount.Equals(existingTransaction.TotalAmount)));

        Assert.NotNull(transactionDtos.FirstOrDefault(t => t.TransactionDate.Equals(request[0].TransactionDate)));
        Assert.NotNull(transactionDtos.FirstOrDefault(t => t.TransactionDate.Equals(request[1].TransactionDate)));
        Assert.NotNull(transactionDtos.FirstOrDefault(t => t.TransactionDate.Equals(request[2].TransactionDate)));
    }

    [Fact]
    public async Task CanCall_CopyPurchaseTransaction_WithValidRequest_ReturnsOk()
    {
        var testUserId = await GetTestUserObjectIdAsync();

        var existingTransaction = await TransactionDocumentsRepository
            .FirstOrDefaultDocumentAsync(t =>
                t.UserId == testUserId &&
                t.TransactionType.Equals(TransactionEnums.TransactionKeys.PURCHASE));

        if (existingTransaction is null)
            Assert.Fail("Need to add transactions before running this test");

        var request = new CopyTransactionRequest[]
        {
            new CopyTransactionRequest(2019, 1, 1),
            new CopyTransactionRequest(2020, 1, 1),
            new CopyTransactionRequest(2021, 1, 1),
        };

        var response = await XpnssApiClient.PostAsJsonAsync(
            ENDPOINTS_TRANSACTIONS_COPY_TRANSACTION
                .Replace("{userId}", testUserId.ToString())
                .Replace("{transactionId}", existingTransaction.Id.ToString()),
            request);

        Assert.NotNull(response);

        var content = await response.Content.ReadAsStringAsync();

        Assert.NotNull(content);

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);
        }

        var transactionDtos =
            (await response.Content.ReadFromJsonAsync<IEnumerable<PurchaseTransactionDto>>())?.ToList() ??
            [];

        Assert.NotNull(transactionDtos);
        Assert.NotEmpty(transactionDtos);

        Assert.True(transactionDtos.All(t => t is PurchaseTransactionDto));

        // RefTransactionId must be the same as the existing transaction
        Assert.True(transactionDtos.All(t =>
            t.RefTransactionId.Equals(existingTransaction.Id.ToString())));

        Assert.True(transactionDtos.All(t =>
            t.UserId.Equals(existingTransaction.UserId.ToString())));

        Assert.True(transactionDtos.All(t =>
            t.AccountId.Equals(existingTransaction.AccountId.ToString())));

        Assert.True(transactionDtos.All(t =>
            t.Description.Equals(existingTransaction.Description)));

        Assert.True(transactionDtos.All(t =>
            t.ExtTransactionId.Equals(existingTransaction.ExtTransactionId)));

        Assert.True(transactionDtos.All(t =>
            t.IsCredit.Equals(existingTransaction.IsCredit)));

        Assert.True(transactionDtos.All(t =>
            t.PayerPayeeId.Equals(existingTransaction.PayerPayeeId?.ToString())));

        Assert.True(transactionDtos.All(t =>
            t.Tags.SequenceEqual(existingTransaction.Tags)));

        Assert.True(transactionDtos.All(t =>
            t.TotalAmount.Equals(existingTransaction.TotalAmount)));

        Assert.NotNull(transactionDtos.FirstOrDefault(t => t.TransactionDate.Equals(request[0].TransactionDate)));
        Assert.NotNull(transactionDtos.FirstOrDefault(t => t.TransactionDate.Equals(request[1].TransactionDate)));
        Assert.NotNull(transactionDtos.FirstOrDefault(t => t.TransactionDate.Equals(request[2].TransactionDate)));

        foreach (var transactionDto in transactionDtos)
        {
            var purchaseDto = Assert.IsType<PurchaseTransactionDto>(transactionDto);

            Assert.Equal(existingTransaction.Items.Count, purchaseDto.Items.Count);

            foreach (var existingItem in existingTransaction.Items)
            {
                Assert.NotNull(purchaseDto.Items.Select(t => t.Amount.Equals(existingItem.Amount)));
                Assert.NotNull(purchaseDto.Items.Select(t => t.CategoryId.Equals(existingItem.CategoryId.Value)));
                Assert.NotNull(purchaseDto.Items.Select(t => t.SubCategoryId.Equals(existingItem.SubCategoryId.Value)));
                Assert.NotNull(purchaseDto.Items.Select(t => t.Description.Equals(existingItem.Description)));
            }
        }
    }
}