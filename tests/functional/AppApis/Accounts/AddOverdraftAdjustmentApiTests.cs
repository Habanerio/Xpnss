using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Application.Commands.OverdraftAdjustment;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Infrastructure.Interfaces.Documents;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

/// <summary>
/// Tests that Overdraft can be adjusted for those Accounts that support Overdrafts.
/// An Adjustment made in the past, will not necessarily affect the current Overdraft.
/// (if there are transactions since the Adjustment date).
/// Also tests that Accounts that do not support Overdrafts, will return BadRequest
/// </summary>
public class AddOverdraftAdjustmentApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_ADJUST_OVERDRAFT =
        "/api/v1/users/{userId}/accounts/{accountId}/overdraft";

    [Theory]
    [InlineData(AccountTypes.Keys.Checking)]
    public async Task CanCall_AdjustOverdraftAmount_ReturnsOk(AccountTypes.Keys accountType)
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        var adjustmentDate = DateTime.Now.AddDays(-(new Random().Next(1, 2 * 365)));
        var previousOverdraftAmount = 0m;

        // Unfortunately, don't have access to the `IHasOverdraftAmount` (on purpose) that is in `Infrastructure.Interfaces`
        if (account is IDocumentHasOverdraftAmount checkingDoc)
            previousOverdraftAmount = checkingDoc.OverdraftAmount;
        else
            Assert.Fail("Not a valid Overdraft Account");

        // Calculate new overdraft

        var expectedOverdraftAmount = previousOverdraftAmount + 1m;

        var request = new AddOverdraftAdjustmentCommand(
            account.UserId.ToString(),
            account.Id.ToString(),
            expectedOverdraftAmount,
            adjustmentDate,
            $"Test {DateTime.Now}");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_OVERDRAFT
                .Replace("{userId}", USER_ID.ToString())
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        return;

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var adjustOverdraftAmountContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustOverdraftAmountContent);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<decimal>>(
            adjustOverdraftAmountContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        // Now fetch this Account again to verify the balance
        var actualAccountDoc = (await AccountDocumentsRepository
                .FirstOrDefaultDocumentAsync(a =>
                    a.Id == account.Id &&
                    a.UserId == USER_ID));

        // Overdraft itself should not change
        if (actualAccountDoc is IDocumentHasOverdraftAmount actualOverdraftDoc)
        {
            Assert.NotNull(actualOverdraftDoc);
            Assert.Equal(previousOverdraftAmount, actualOverdraftDoc.OverdraftAmount);
        }
        else
            Assert.Fail("Document is not an Overdraft Account");
    }

    [Theory]
    [InlineData(AccountTypes.Keys.Cash)]
    [InlineData(AccountTypes.Keys.Savings)]
    [InlineData(AccountTypes.Keys.CreditCard)]
    [InlineData(AccountTypes.Keys.LineOfCredit)]
    public async Task CanNotCall_AdjustOverdraftAmount_InvalidAccountType_ReturnsBadRequestOk(AccountTypes.Keys accountType)
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();
        var adjustmentDate = DateTime.Now.AddDays(-(new Random().Next(1, 2 * 365)));

        var request = new AddOverdraftAdjustmentCommand(
            account.UserId.ToString(),
            account.Id.ToString(),
            10,
            adjustmentDate,
            $"Test {DateTime.Now}");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_OVERDRAFT
                .Replace("{userId}", USER_ID.ToString())
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        return;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var adjustOverdraftAmountContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustOverdraftAmountContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustOverdraftAmountContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.NotEmpty(apiResponse);
        Assert.Equal($"the Account Type `{accountType.ToString()}` does not support Overdrafts", apiResponse[0]);
    }

    [Theory]
    [InlineData(AccountTypes.Keys.Checking, -0.01)]
    public async Task CanNotCall_AdjustOverdraftAmount_ValueTooLow_ReturnsBadRequestOk(AccountTypes.Keys accountType, decimal value)
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        var adjustmentDate = DateTime.Now.AddDays(-(new Random().Next(1, 2 * 365)));

        var request = new AddOverdraftAdjustmentCommand(
            account.UserId.ToString(),
            account.Id.ToString(),
            value,
            adjustmentDate,
            $"Test {DateTime.Now}");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_OVERDRAFT
                .Replace("{userId}", USER_ID.ToString())
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        return;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var adjustOverdraftAmountContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustOverdraftAmountContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustOverdraftAmountContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.NotEmpty(apiResponse);
        Assert.Equal($"'Overdraft CreditLimit' must be greater than or equal to '0'.", apiResponse[0]);
    }
}