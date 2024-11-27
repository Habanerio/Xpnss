using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Setup;

public class SetupApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_SETUP = "/api/v1/setup/{userId}";

    [Fact]
    public async Task CanCall_Setup_WithValidRequest_ReturnsOk()
    {
        // Act
        var response = await HttpClient.PostAsync(
            ENDPOINTS_SETUP
                .Replace("{userId}", USER_ID),
            null);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

    }

    [Fact]
    public async Task CanCall_Setup_RandomUser_WithValidRequest_ReturnsOk()
    {
        // Act
        var response = await HttpClient.PostAsync(
            ENDPOINTS_SETUP
                .Replace("{userId}", Guid.NewGuid().ToString()),
            null);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

    }
}