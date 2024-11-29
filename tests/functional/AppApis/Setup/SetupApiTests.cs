using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Setup;

public class SetupApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_SETUP = "/api/v1/setup";

    [Fact]
    public async Task CanCall_Setup_WithValidRequest_ReturnsOk()
    {
        var email = TEST_USER_EMAIL;
        var firstName = "Test";
        var lastName = "User";
        var extUserId = "test-user";


        var createUserProfileRequest = new CreateUserProfileRequest(email, firstName, lastName, extUserId);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_SETUP,
            createUserProfileRequest);

        response.EnsureSuccessStatusCode();

        var setUpUserProfileDtoContent = await response.Content.ReadAsStringAsync();

        var userProfile = JsonSerializer.Deserialize<UserProfileDto>(
            setUpUserProfileDtoContent,
            JsonSerializationOptions);

        // Assert
        Assert.NotNull(userProfile);
        Assert.NotEmpty(userProfile.Id);
        Assert.Equal(email, userProfile.Email);
        Assert.Equal(firstName, userProfile.FirstName);
        Assert.Equal(lastName, userProfile.LastName);
        Assert.Equal(extUserId, userProfile.ExtUserId);
    }
}