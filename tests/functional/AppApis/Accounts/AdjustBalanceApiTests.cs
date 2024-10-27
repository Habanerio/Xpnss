using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

/*
public class AdjustBalanceApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_ADJUST_BALANCE = "/api/v1/users/{userId}/accounts/{accountId}/balance";
    private const string ENDPOINTS_ACCOUNTS_GET_ACCOUNT = "/api/v1/users/{userId}/accounts/{accountId}";
    private const string ENDPOINTS_ACCOUNTS_GET_ACCOUNTS = "/api/v1/users/{userId}/accounts";

    [Fact]
    public async Task AdjustBalance_ReturnsOk()
    {
        // Get all accounts so that we can pick one to adjust its balance
        var accountsResponse = await HttpClient.GetAsync(
            ENDPOINTS_ACCOUNTS_GET_ACCOUNTS.Replace("{userId}",
                USER_ID));

        accountsResponse.EnsureSuccessStatusCode();

        var accountsContent = await accountsResponse.Content.ReadAsStringAsync();
        var accountsApiResponse = JsonSerializer.Deserialize<ApiResponse<List<AccountDto>>>(
            accountsContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var accounts = accountsApiResponse?.Data?.ToList() ?? [];

        // Take the first (maybe make random)
        var account = accounts[0];

        // Calculate new balance
        var previousBalance = account.Balance;
        var expectedBalance = previousBalance + 100m;

        var adjustBalanceRequest = new
        {
            UserId = account.UserId,
            AccountId = account.Id,
            NewBalance = expectedBalance,
            Reason = $"Test {DateTime.Now}"
        };

        // Act
        var adjustBalanceResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_BALANCE
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id),
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
        Assert.NotNull(apiResponse.Data);

        Assert.Equal(expectedBalance, apiResponse.Data);

        // Not fetch this account again to verify the balance
        var accountResponse = await HttpClient.GetAsync(
            ENDPOINTS_ACCOUNTS_GET_ACCOUNT
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id));

        accountResponse.EnsureSuccessStatusCode();

        var accountContent = await accountResponse.Content.ReadAsStringAsync();
        var accountApiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(
            accountContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        Assert.NotNull(accountApiResponse);
        Assert.True(accountApiResponse.IsSuccess);
        Assert.NotNull(accountApiResponse.Data);
        Assert.Equal(expectedBalance, accountApiResponse.Data.Balance);
    }
}
*/