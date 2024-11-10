using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.Accounts.Commands.AdjustInterestRate;
using Habanerio.Xpnss.Domain.Accounts;
using Habanerio.Xpnss.Infrastructure.Documents;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class AdjustInteretRateApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
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
            .FindAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        AccountDocument? creditAccount = null;

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

        var adjustCreditLimitRequest = new AdjustInterestRateCommand(
            account.UserId,
            account.Id.ToString(),
            expectedInterestRate,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var adjustInterestRateResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_INTEREST_RATE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            adjustCreditLimitRequest);

        adjustInterestRateResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, adjustInterestRateResponse.StatusCode);

        var adjustInterestRateContent = await adjustInterestRateResponse.Content.ReadAsStringAsync();
        Assert.NotNull(adjustInterestRateContent);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<decimal>>(
            adjustInterestRateContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        // Now fetch this account again to verify the balance
        var actualAccountDoc = (await AccountDocumentsRepository
                .FirstOrDefaultAsync(a =>
                    a.Id == account.Id &&
                    a.UserId == USER_ID));

        if (actualAccountDoc is CreditCardAccountDocument actualCcDoc)
        {
            Assert.NotNull(actualCcDoc);
            Assert.Equal(expectedInterestRate, actualCcDoc.InterestRate);
        }
        else if (actualAccountDoc is LineOfCreditAccountDocument actualLocDoc)
        {
            Assert.NotNull(actualLocDoc);
            Assert.Equal(expectedInterestRate, actualLocDoc.InterestRate);
        }
        else if (actualAccountDoc is SavingsAccountDocument actualSavingsDoc)
        {
            Assert.NotNull(actualSavingsDoc);
            Assert.Equal(expectedInterestRate, actualSavingsDoc.InterestRate);
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
            .FindAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        var adjustInterestRateRequest = new AdjustInterestRateCommand(
            account.UserId,
            account.Id.ToString(),
            10,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var adjustInterestRateResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_INTEREST_RATE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            adjustInterestRateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, adjustInterestRateResponse.StatusCode);

        var adjustInterestRateContent = await adjustInterestRateResponse.Content.ReadAsStringAsync();
        Assert.NotNull(adjustInterestRateContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustInterestRateContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(apiResponse);
        Assert.NotEmpty(apiResponse);
        Assert.Equal($"The Account Type `{accountType.ToString()}` does not support Interest Rates", apiResponse[0]);
    }

    [Theory]
    [InlineData(AccountTypes.Keys.CreditCard, -0.01)]
    [InlineData(AccountTypes.Keys.LineOfCredit, -0.01)]
    public async Task CanNotCall_AdjustInterestRate_ValueTooLow_ReturnsBadRequestOk(AccountTypes.Keys accountType, decimal value)
    {
        var accounts = await AccountDocumentsRepository
            .FindAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        var adjustInterestRateRequest = new AdjustInterestRateCommand(
            account.UserId,
            account.Id.ToString(),
            value,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var adjustInterestRateResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_INTEREST_RATE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            adjustInterestRateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, adjustInterestRateResponse.StatusCode);

        var adjustInterestRateContent = await adjustInterestRateResponse.Content.ReadAsStringAsync();
        Assert.NotNull(adjustInterestRateContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustInterestRateContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
            .FindAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        var adjustInterestRateRequest = new AdjustInterestRateCommand(
            account.UserId,
            account.Id.ToString(),
            value,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var adjustInterestRateResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_INTEREST_RATE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            adjustInterestRateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, adjustInterestRateResponse.StatusCode);

        var adjustInterestRateContent = await adjustInterestRateResponse.Content.ReadAsStringAsync();
        Assert.NotNull(adjustInterestRateContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustInterestRateContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(apiResponse);
        Assert.NotEmpty(apiResponse);
        Assert.Equal($"'Interest Rate' must be less than or equal to '100'.", apiResponse[0]);
    }
}