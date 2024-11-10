using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.Accounts.Commands.AdjustCreditLimit;
using Habanerio.Xpnss.Domain.Accounts;
using Habanerio.Xpnss.Infrastructure.Documents;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class AdjustCreditLimitApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_ADJUST_CREDIT_LIMIT =
        "/api/v1/users/{userId}/accounts/{accountId}/credit-limit";

    [Theory]
    [InlineData(AccountTypes.Keys.CreditCard)]
    [InlineData(AccountTypes.Keys.LineOfCredit)]
    public async Task CanCall_AdjustCreditLimit_ReturnsOk(AccountTypes.Keys accountType)
    {
        var accounts = await AccountDocumentsRepository
            .FindAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        CreditAccountDocument? creditAccount = null;

        if (account.AccountType == AccountTypes.Keys.CreditCard)
            creditAccount = (CreditCardAccountDocument)account;
        else if (account.AccountType == AccountTypes.Keys.LineOfCredit)
            creditAccount = (LineOfCreditAccountDocument)account;
        else
            throw new InvalidOperationException("Not a valid Credit Account");

        // Calculate new credit limit
        var previousCreditLimit = creditAccount.CreditLimit;
        var expectedCreditLimit = previousCreditLimit + 100m;

        var adjustCreditLimitRequest = new AdjustCreditLimitCommand(
            account.UserId,
            account.Id.ToString(),
            expectedCreditLimit,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var adjustCreditLimitResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_CREDIT_LIMIT
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            adjustCreditLimitRequest);

        adjustCreditLimitResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, adjustCreditLimitResponse.StatusCode);

        var adjustCreditLimitContent = await adjustCreditLimitResponse.Content.ReadAsStringAsync();
        Assert.NotNull(adjustCreditLimitContent);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<decimal>>(
            adjustCreditLimitContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        // Now fetch this account again to verify the balance
        var actualAccountDoc = (await AccountDocumentsRepository
                .FirstOrDefaultAsync(a =>
                    a.Id == account.Id &&
                    a.UserId == USER_ID))
            as CreditAccountDocument;

        Assert.NotNull(actualAccountDoc);
        Assert.Equal(expectedCreditLimit, actualAccountDoc.CreditLimit);
    }

    [Theory]
    [InlineData(AccountTypes.Keys.Cash)]
    [InlineData(AccountTypes.Keys.Checking)]
    [InlineData(AccountTypes.Keys.Savings)]
    public async Task CanNotCall_AdjustCreditLimit_InvalidAccountType_ReturnsBadRequestOk(AccountTypes.Keys accountType)
    {
        var accounts = await AccountDocumentsRepository
            .FindAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        var adjustCreditLimitRequest = new AdjustCreditLimitCommand(
            account.UserId,
            account.Id.ToString(),
            100000000000000,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var adjustCreditLimitResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_CREDIT_LIMIT
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            adjustCreditLimitRequest);

        Assert.Equal(HttpStatusCode.BadRequest, adjustCreditLimitResponse.StatusCode);

        var adjustCreditLimitContent = await adjustCreditLimitResponse.Content.ReadAsStringAsync();
        Assert.NotNull(adjustCreditLimitContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustCreditLimitContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(apiResponse);
        Assert.NotEmpty(apiResponse);
        Assert.Equal($"The Account Type `{accountType.ToString()}` does not support Credit Limits", apiResponse[0]);
    }
}