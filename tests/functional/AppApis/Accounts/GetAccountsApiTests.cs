using System.Text.Json;
using Habanerio.Xpnss.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class GetAccountsApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    [Fact]
    public async Task GetAccounts_ReturnsOk()
    {
        var userId = await GetTestUserObjectIdAsync();

        // Act
        var response = await HttpClient.GetAsync(
            ENDPOINTS_ACCOUNTS_GET_ACCOUNTS.Replace("{userId}", userId.ToString()));

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);

        var apiResponse = JsonSerializer.Deserialize<IEnumerable<AccountDto>>(
            content,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);

        var accounts = Assert.IsType<List<AccountDto>>(apiResponse);

        Assert.True(accounts.TrueForAll(a =>
            a.UserId == userId.ToString()));
    }
}