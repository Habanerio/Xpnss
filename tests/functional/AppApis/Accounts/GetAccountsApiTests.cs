using System.Net;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class GetAccountsApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_GET_ACCOUNTS = "/api/v1/users/{userId}/accounts";

    [Fact]
    public async Task GetAccounts_ReturnsOk()
    {
        // Act
        var response = await HttpClient.GetAsync(
            ENDPOINTS_ACCOUNTS_GET_ACCOUNTS.Replace("{userId}",
                USER_ID));

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<AccountDto>>>(
            content,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        var accounts = Assert.IsType<List<AccountDto>>(apiResponse.Data);

        Assert.True(accounts.TrueForAll(a => a.UserId == USER_ID));
    }
}