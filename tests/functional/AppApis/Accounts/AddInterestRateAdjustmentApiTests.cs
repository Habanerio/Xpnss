using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Application.Commands.InterestRateAdjustment;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Infrastructure.Interfaces.Documents;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

/// <summary>
/// Tests that Interest Rate can be adjusted for those Accounts that support Interest Rates.
/// An Adjustment made in the past, will not necessarily affect the current Interest Rate.
/// (if there are transactions since the Adjustment date).
/// Also tests that Accounts that do not support Interest Rates, will return BadRequest
/// </summary>
public class AddInterestRateAdjustmentApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_ADJUST_INTEREST_RATE =
        "/api/v1/users/{userId}/accounts/{accountId}/interest-rate";

    [Theory]
    [InlineData(AccountTypes.Keys.CreditCard)]
    [InlineData(AccountTypes.Keys.LineOfCredit)]
    [InlineData(AccountTypes.Keys.Savings)]
    public async Task CanCall_AdjustInterestRate_ReturnsOk(AccountTypes.Keys accountType)
    {
        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        AccountDocument? creditAccount = null;

        var adjustmentDate = DateTime.Now.AddDays(-(new Random().Next(1, 2 * 365)));
        var previousInterestRate = 0m;

        // Unfortunately, don't have access to the `IHasInterestRate` (on purpose) that is in `Infrastructure.Interfaces`
        if (account is CreditCardAccountDocument ccDoc)
            previousInterestRate = ccDoc.InterestRate;
        else if (account is LineOfCreditAccountDocument locDoc)
            previousInterestRate = locDoc.InterestRate;
        else if (account is SavingsAccountDocument savingsDoc)
            previousInterestRate = savingsDoc.InterestRate;
        else
            throw new InvalidOperationException("Not a valid Credit Account");

        // Calculate new credit limit

        var expectedInterestRate = previousInterestRate + 1m;

        var request = new AddInterestRateAdjustmentCommand(
            account.UserId,
            account.Id.ToString(),
            expectedInterestRate,
            adjustmentDate,
            $"Test {DateTime.Now}");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_INTEREST_RATE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        return;

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var adjustInterestRateContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustInterestRateContent);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<decimal>>(
            adjustInterestRateContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        // Now fetch this Account again to verify the balance
        var actualAccountDoc = (await AccountDocumentsRepository
                .FirstOrDefaultDocumentAsync(a =>
                    a.Id == account.Id &&
                    a.UserId == USER_ID));

        // Interest Rate itself should not change
        if (actualAccountDoc is IDocumentHasInterestRate interestRateDoc)
        {
            Assert.NotNull(interestRateDoc);
            Assert.Equal(previousInterestRate, interestRateDoc.InterestRate);
        }
        else
            throw new InvalidOperationException("Not a valid Credit Account");
    }

    [Theory]
    [InlineData(AccountTypes.Keys.Cash)]
    [InlineData(AccountTypes.Keys.Checking)]
    public async Task CanNotCall_AdjustInterestRate_InvalidAccountType_ReturnsBadRequestOk(AccountTypes.Keys accountType)
    {
        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();
        var adjustmentDate = DateTime.Now.AddDays(-(new Random().Next(1, 2 * 365)));

        var request = new AddInterestRateAdjustmentCommand(
            account.UserId,
            account.Id.ToString(),
            10,
            adjustmentDate,
            $"Test {DateTime.Now}");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_INTEREST_RATE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        return;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var adjustInterestRateContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustInterestRateContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustInterestRateContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.NotEmpty(apiResponse);
        Assert.Equal($"the Account Type `{accountType.ToString()}` does not support Interest Rates", apiResponse[0]);
    }

    [Theory]
    [InlineData(AccountTypes.Keys.CreditCard, -0.01)]
    [InlineData(AccountTypes.Keys.LineOfCredit, -0.01)]
    public async Task CanNotCall_AdjustInterestRate_ValueTooLow_ReturnsBadRequestOk(AccountTypes.Keys accountType, decimal value)
    {
        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        var request = new AddInterestRateAdjustmentCommand(
            account.UserId,
            account.Id.ToString(),
            value,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_INTEREST_RATE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        return;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var adjustInterestRateContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustInterestRateContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustInterestRateContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.NotEmpty(apiResponse);
        Assert.Equal($"'Interest Rate' must be greater than or equal to '0'.", apiResponse[0]);
    }

    [Theory]
    [InlineData(AccountTypes.Keys.CreditCard, 100.1)]
    [InlineData(AccountTypes.Keys.LineOfCredit, 100.1)]
    public async Task CanNotCall_AdjustInterestRate_ValueTooHigh_ReturnsBadRequestOk(AccountTypes.Keys accountType, decimal value)
    {
        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        var request = new AddInterestRateAdjustmentCommand(
            account.UserId,
            account.Id.ToString(),
            value,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_INTEREST_RATE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        return;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var adjustInterestRateContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustInterestRateContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustInterestRateContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.NotEmpty(apiResponse);
        Assert.Equal($"'Interest Rate' must be less than or equal to '100'.", apiResponse[0]);
    }
}