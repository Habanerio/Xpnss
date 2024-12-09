using System.Net.Http.Json;
using System.Text.Json;

using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.Types;
using Microsoft.AspNetCore.Mvc.Testing;

using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Register;

public class RegisterApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    [Fact]
    public async Task CanCall_Setup_WithValidRequest_ReturnsOk()
    {
        var email = TEST_USER_EMAIL;
        var firstName = "Test";
        var lastName = "User";
        var extUserId = "test-user";


        var createUserProfileRequest = new CreateUserProfileApiRequest(
            email,
            firstName,
            lastName,
            extUserId,
            CurrencyEnums.CurrencyKeys.USD);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_REGISTER,
            createUserProfileRequest);

        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

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

        var accountDocs = (await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == ObjectId.Parse(userProfile.Id)))?.ToList() ?? [];

        Assert.NotNull(accountDocs);
        Assert.NotEmpty(accountDocs);
    }

    /// <summary>
    /// Tests that we can create a (random) user profile.
    /// And that default Accounts and Categories are assigned to them via the Integration Events
    /// </summary>
    [Fact]
    public async Task CanCall_Setup_Random_WithValidRequest_ReturnsOk()
    {
        var email = $"{Guid.NewGuid()}@test.com";
        var firstName = $"Random";
        var lastName = Guid.NewGuid().ToString();
        var extUserId = Guid.NewGuid().ToString();

        var createUserProfileRequest = new CreateUserProfileApiRequest(
            email,
            firstName,
            extUserId,
            lastName);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_REGISTER,
            createUserProfileRequest);

        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.Fail(content);
        }

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

        var accountDocs = (await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == ObjectId.Parse(userProfile.Id)))?.ToList() ?? [];

        Assert.NotNull(accountDocs);
        Assert.NotEmpty(accountDocs);
        Assert.True(accountDocs.Count > 0);

        var categoryDocs = (await CategoryDocumentsRepository
            .FindDocumentsAsync(c =>
                c.UserId == ObjectId.Parse(userProfile.Id)))?.ToList() ?? [];

        Assert.NotNull(categoryDocs);
        Assert.NotEmpty(categoryDocs);

        foreach (var categoryDoc in categoryDocs)
        {
            Assert.Equal(userProfile.Id, categoryDoc.UserId.ToString());
            Assert.NotEmpty(categoryDoc.SubCategories);
        }
    }
}