using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class CreateAccountApiTests : BaseFunctionalApisTests,
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_CREATE_ACCOUNT = "/api/v1/users/{userId}/accounts";

    public CreateAccountApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
        base(factory)
    { }

    [Fact]
    public async Task CreateAccount_CashAccount_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateAccountEndpoint.Request
        {
            UserId = USER_ID,
            AccountTypeId = (int)AccountType.Cash,
            Name = "Test Cash Account",
            Description = "Test Cash Account Description",
            Balance = 0,
            DisplayColor = "#000000"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT
                .Replace("{userId}", USER_ID),
            request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(apiResponse);
        Assert.True(!string.IsNullOrWhiteSpace(apiResponse.Data));
        Assert.True(apiResponse.IsSuccess);
    }

    [Fact]
    public async Task CreateAccount_CheckingAccount_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateAccountEndpoint.Request
        {
            UserId = USER_ID,
            AccountTypeId = (int)AccountType.Checking,
            Name = "Test Checking Account",
            Description = "Test Checking Account Description",
            Balance = 0,
            DisplayColor = "#000000"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(apiResponse);
        Assert.True(!string.IsNullOrWhiteSpace(apiResponse.Data));
        Assert.True(apiResponse.IsSuccess);
    }

    [Fact]
    public async Task CreateAccount_CreditCardAccount_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateAccountEndpoint.Request
        {
            UserId = USER_ID,
            AccountTypeId = (int)AccountType.CreditCard,
            Name = "Test Credit Card Account",
            Description = "Test Credit Card Account Description",
            Balance = 0,
            CreditLimit = 1000,
            InterestRate = 10,
            DisplayColor = "#000000"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(apiResponse);
        Assert.True(!string.IsNullOrWhiteSpace(apiResponse.Data));
        Assert.True(apiResponse.IsSuccess);
    }

    [Fact]
    public async Task CreateAccount_WithInvalidRequest_NULL_ReturnsBadRequest()
    {
        // Arrange
        CreateAccountEndpoint.Request? request = null;

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
    }

    [Fact]
    public async Task CreateAccount_WithInvalidRequest_EmptyUserId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountEndpoint.Request
        {
            UserId = string.Empty,
            AccountTypeId = (int)AccountType.Cash,
            Name = "Test Cash Account",
            Description = "Test Cash Account Description",
            Balance = 0,
            DisplayColor = "#000000"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal(1, errors.Count);
        Assert.Equal("'User Id' must not be empty.", errors[0]);
    }

    [Fact]
    public async Task CreateAccount_WithInvalidRequest_EmptyAccountType_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountEndpoint.Request
        {
            UserId = USER_ID,
            //AccountTypeId = (int)AccountType.Cash,
            Name = "Test Cash Account",
            Description = "Test Cash Account Description",
            Balance = 0,
            DisplayColor = "#000000"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal(1, errors.Count);
        Assert.Equal("'Account Type Id' must not be empty.", errors[0]);
    }

    [Fact]
    public async Task CreateAccount_WithInvalidRequest_EmptyName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountEndpoint.Request
        {
            UserId = USER_ID,
            AccountTypeId = (int)AccountType.Cash,
            //Name = "Test Cash Account",
            Description = "Test Cash Account Description",
            Balance = 0,
            DisplayColor = "#000000"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal(1, errors.Count);
        Assert.Equal("'Name' must not be empty.", errors[0]);
    }

    [Fact]
    public async Task CreateAccount_WithInvalidRequest_CreditLimit_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountEndpoint.Request
        {
            UserId = USER_ID,
            AccountTypeId = (int)AccountType.CreditCard,
            Name = "Test Cash Account",
            Description = "Test Cash Account Description",
            Balance = 0,
            CreditLimit = -1,
            DisplayColor = "#000000"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal(1, errors.Count);
        Assert.Equal("'Credit Limit' must be greater than or equal to '0'.", errors[0]);
    }

    [Fact]
    public async Task CreateAccount_WithInvalidRequest_InterestRate_BelowZero_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountEndpoint.Request
        {
            UserId = USER_ID,
            AccountTypeId = (int)AccountType.CreditCard,
            Name = "Test Cash Account",
            Description = "Test Cash Account Description",
            Balance = 0,
            CreditLimit = 1000,
            InterestRate = -1,
            DisplayColor = "#000000"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal(1, errors.Count);
        Assert.Equal("'Interest Rate' must be greater than or equal to '0'.", errors[0]);
    }

    [Fact]
    public async Task CreateAccount_WithInvalidRequest_InterestRate_AboveHundred_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountEndpoint.Request
        {
            UserId = USER_ID,
            AccountTypeId = (int)AccountType.CreditCard,
            Name = "Test Cash Account",
            Description = "Test Cash Account Description",
            Balance = 0,
            CreditLimit = 1000,
            InterestRate = 101,
            DisplayColor = "#000000"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal(1, errors.Count);
        Assert.Equal("'Interest Rate' must be less than or equal to '100'.", errors[0]);
    }
}