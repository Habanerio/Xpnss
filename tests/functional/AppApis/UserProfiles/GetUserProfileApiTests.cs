using System.Net;
using System.Text.Json;
using Habanerio.Xpnss.Shared.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.UserProfiles;

public class GetUserProfileApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    [Fact]
    public async Task CanCall_GetUserProfile_WithValidRequest_ReturnsOk()
    {
        var actualUserProfileDocument = await UserProfileDocumentsRepository
            .FirstOrDefaultDocumentAsync(u => true);

        if (actualUserProfileDocument is null)
            Assert.Fail("Cannot find a UserProfileDocument for the test");

        var userId = actualUserProfileDocument.Id;

        // Act
        var response = await HttpClient.GetAsync(
            ENDPOINTS_USER_PROFILES_GET_USER_PROFILE
                .Replace("{userId}", userId.ToString()));

        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<UserProfileDto>(
            content,
            JsonSerializationOptions);

        // Assert
        Assert.NotNull(apiResponse);

        var actualDto = Assert.IsType<UserProfileDto>(apiResponse);

        Assert.NotNull(actualDto);
        Assert.Equal(userId.ToString(), actualDto.Id);
        Assert.Equal(actualUserProfileDocument.ExtUserId, actualDto.ExtUserId);
        Assert.Equal(actualUserProfileDocument.FirstName, actualDto.FirstName);
        Assert.Equal(actualUserProfileDocument.LastName, actualDto.LastName);
        Assert.Equal(actualUserProfileDocument.Email, actualDto.Email);
    }

    [Fact]
    public async Task CannotCall_GetUserProfile_WithInvalid_UserId_ReturnsNotFound()
    {
        var userId = ObjectId.GenerateNewId();

        // Act
        var response = await HttpClient.GetAsync(
            ENDPOINTS_USER_PROFILES_GET_USER_PROFILE
            .Replace("{userId}", userId.ToString()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}