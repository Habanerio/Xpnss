using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Application.Commands.CreateAccount;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class CreateAccountApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_CREATE_ACCOUNT = "/api/v1/users/{userId}/accounts";


    [Fact]
    public async Task CanCall_CreateAccount_CashAccount_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateAccountCommand(
            USER_ID,
            AccountTypes.Keys.Cash.ToString(),
            "Test Cash Account",
            "Test Cash Account Description", DisplayColor: "#123ABC");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT
                .Replace("{userId}", USER_ID),
            request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID, accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.Cash.ToString(), accountDto.AccountType);
        Assert.Equal(request.Name, accountDto.Name);
        Assert.Equal(0, accountDto.Balance);
        Assert.Equal(request.Description, accountDto.Description);
        Assert.Equal(request.DisplayColor, accountDto.DisplayColor);
        Assert.Equal(DateTime.UtcNow, accountDto.DateCreated, TimeSpan.FromSeconds(5));
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);
        Assert.True(!accountDto.IsCredit);
        Assert.True(!accountDto.IsDeleted);

        Assert.Equal(0, request.CreditLimit);
        Assert.Equal(0, request.InterestRate);
        Assert.Equal(0, request.OverdraftAmount);
    }

    [Fact]
    public async Task CanCall_CreateAccount_CheckingAccount_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            USER_ID,
            AccountTypes.Keys.Checking.ToString(),
            "Test Checking Account",
            "Test Checking Account Description",
            OverdraftAmount: 4232, DisplayColor: "#0df000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID, accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.Checking.ToString(), accountDto.AccountType);
        Assert.Equal(request.Name, accountDto.Name);
        Assert.Equal(0, accountDto.Balance);
        Assert.Equal(request.Description, accountDto.Description);
        Assert.Equal(request.DisplayColor, accountDto.DisplayColor);
        Assert.Equal(request.OverdraftAmount, accountDto.OverdraftAmount);
        Assert.Equal(DateTime.UtcNow, accountDto.DateCreated, TimeSpan.FromSeconds(5));
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);
        Assert.True(!accountDto.IsCredit);
        Assert.True(!accountDto.IsDeleted);

        Assert.Equal(0, request.CreditLimit);
        Assert.Equal(0, request.InterestRate);
    }

    [Fact]
    public async Task CanCall_CreateAccount_SavingsAccount_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID,
            AccountType: AccountTypes.Keys.Savings.ToString(),
            Name: "Test Savings Account",
            Description: "Test Savings Account Description",
            InterestRate: 10, DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID, accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.Savings.ToString(), accountDto.AccountType);
        Assert.Equal(request.Name, accountDto.Name);
        Assert.Equal(0, accountDto.Balance);
        Assert.Equal(request.Description, accountDto.Description);
        Assert.Equal(request.DisplayColor, accountDto.DisplayColor);
        Assert.Equal(request.InterestRate, accountDto.InterestRate);
        Assert.Equal(DateTime.UtcNow, accountDto.DateCreated, TimeSpan.FromSeconds(5));
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);
        Assert.True(!accountDto.IsCredit);
        Assert.True(!accountDto.IsDeleted);

        Assert.Equal(0, request.CreditLimit);
        Assert.Equal(0, request.OverdraftAmount);
    }

    [Fact]
    public async Task CanCall_CreateAccount_CreditCardAccount_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID,
            AccountType: AccountTypes.Keys.CreditCard.ToString(),
            Name: "Test Credit Card Account",
            Description: "Test Credit Card Account Description",
            CreditLimit: 1000,
            InterestRate: 10, DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID, accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.CreditCard.ToString(), accountDto.AccountType);
        Assert.Equal(request.Name, accountDto.Name);
        Assert.Equal(0, accountDto.Balance);
        Assert.Equal(request.CreditLimit, accountDto.CreditLimit);
        Assert.Equal(request.Description, accountDto.Description);
        Assert.Equal(request.DisplayColor, accountDto.DisplayColor);
        Assert.Equal(request.InterestRate, accountDto.InterestRate);
        Assert.Equal(DateTime.UtcNow, accountDto.DateCreated, TimeSpan.FromSeconds(5));
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);
        Assert.True(accountDto.IsCredit);
        Assert.False(accountDto.IsDeleted);

        Assert.Equal(0, request.OverdraftAmount);
    }

    [Fact]
    public async Task CanCall_CreateAccount_LineOfCreditAccount_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID,
            AccountType: AccountTypes.Keys.LineOfCredit.ToString(),
            Name: "Test Line Of Credit Account",
            Description: "Test Line Of Credit Account Description",
            CreditLimit: 1000,
            InterestRate: 10, DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID, accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.LineOfCredit.ToString(), accountDto.AccountType);
        Assert.Equal(request.Name, accountDto.Name);
        Assert.Equal(0, accountDto.Balance);
        Assert.Equal(request.CreditLimit, accountDto.CreditLimit);
        Assert.Equal(request.Description, accountDto.Description);
        Assert.Equal(request.DisplayColor, accountDto.DisplayColor);
        Assert.Equal(request.InterestRate, accountDto.InterestRate);
        Assert.Equal(DateTime.UtcNow, accountDto.DateCreated, TimeSpan.FromSeconds(5));
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);
        Assert.True(accountDto.IsCredit);
        Assert.True(!accountDto.IsDeleted);

        Assert.Equal(0, request.OverdraftAmount);
    }


    [Fact]
    public async Task CannotCall_CreateAccount_WithInvalidRequest_NULL_ReturnsBadRequest()
    {
        // Arrange
        CreateAccountCommand? request = null;

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
    }

    [Fact]
    public async Task CannotCall_CreateAccount_WithInvalidRequest_EmptyUserId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: string.Empty,
            AccountType: "Cash",
            Name: "Test Cash Account",
            Description: "Test Cash Account Description", DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);

        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("'User Id' must not be empty.", errors[0]);
    }

    [Fact]
    public async Task CannotCall_CreateAccount_WithInvalidRequest_EmptyAccountType_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID,
            AccountType: string.Empty,
            Name: "Test Cash Account",
            Description: "Test Cash Account Description", DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);

        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("'Account Type' must not be empty.", errors[0]);
    }

    [Fact]
    public async Task CannotCall_CreateAccount_WithInvalidRequest_EmptyName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID,
            AccountType: AccountTypes.Keys.Cash.ToString(),
            Name: string.Empty,
            Description: "Test Cash Account Description",
            DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);

        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("'Name' must not be empty.", errors[0]);
    }

    [Fact]
    public async Task CannotCall_CreateAccount_WithInvalidRequest_CreditLimit_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID,
            AccountType: "CreditCard",
            Name: "Test Cash Account",
            Description: "Test Cash Account Description",
            CreditLimit: -1,
            DisplayColor: "#000000"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);

        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("'Credit Limit' must be greater than or equal to '0'.", errors[0]);
    }

    [Fact]
    public async Task CannotCall_CreateAccount_WithInvalidRequest_InterestRate_BelowZero_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID,
            AccountType: "CreditCard",
            Name: "Test Cash Account",
            Description: "Test Cash Account Description",
            CreditLimit: 1000,
            InterestRate: -1,
            DisplayColor: "#000000"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);

        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("'Interest Rate' must be greater than or equal to '0'.", errors[0]);
    }

    [Fact]
    public async Task CannotCall_CreateAccount_WithInvalidRequest_InterestRate_AboveHundred_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID,
            AccountType: "CreditCard",
            Name: "Test Cash Account",
            Description: "Test Cash Account Description",
            CreditLimit: 1000,
            InterestRate: 101,
            DisplayColor: "#000000"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);

        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("'Interest Rate' must be less than or equal to '100'.", errors[0]);
    }
}