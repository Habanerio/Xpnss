using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.Accounts.Commands.AdjustBalance;
using Habanerio.Xpnss.Domain.Accounts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;


public class AdjustBalanceApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_ADJUST_BALANCE = "/api/v1/users/{userId}/accounts/{accountId}/balance";

    [Theory]
    [InlineData(AccountTypes.Keys.Cash)]
    [InlineData(AccountTypes.Keys.Checking)]
    [InlineData(AccountTypes.Keys.Savings)]
    [InlineData(AccountTypes.Keys.CreditCard)]
    [InlineData(AccountTypes.Keys.LineOfCredit)]
    public async Task CanCall_AdjustBalance_ReturnsOk(AccountTypes.Keys accountType)
    {
        var accounts = await AccountDocumentsRepository
            .FindAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        if (account is null)
            return;

        // Calculate new balance
        var previousBalance = account.Balance;
        var expectedBalance = previousBalance + 100m;

        var adjustBalanceRequest = new AdjustBalanceCommand(
            account.UserId,
            account.Id.ToString(),
            expectedBalance,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var adjustBalanceResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_BALANCE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            adjustBalanceRequest);


        adjustBalanceResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, adjustBalanceResponse.StatusCode);

        var adjustBalanceContent = await adjustBalanceResponse.Content.ReadAsStringAsync();
        Assert.NotNull(adjustBalanceContent);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<decimal>>(
            adjustBalanceContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        Assert.Equal(expectedBalance, apiResponse.Data);

        // Now fetch this account again to verify the balance
        var actualAccountDoc = await AccountDocumentsRepository.FirstOrDefaultAsync(a => a.Id == account.Id && a.UserId == USER_ID);

        Assert.NotNull(actualAccountDoc);
        Assert.Equal(expectedBalance, actualAccountDoc.Balance);
    }
}
