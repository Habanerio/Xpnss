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
    [Fact]
    public async Task CanCall_CreateAccount_CashAccount_WithValidRequest_ReturnsOk()
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand(
            USER_ID.ToString(),
            AccountTypes.Keys.CASH.ToString(),
            "Test Cash Account",
            "Test Cash Account Description", DisplayColor: "#123ABC");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT
                .Replace("{userId}", USER_ID.ToString()),
            request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID.ToString(), accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.CASH.ToString(), accountDto.AccountType);
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            USER_ID.ToString(),
            AccountTypes.Keys.CHECKING.ToString(),
            "Test Checking Account",
            "Test Checking Account Description",
            OverdraftAmount: 4232, DisplayColor: "#0df000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
            request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID.ToString(), accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.CHECKING.ToString(), accountDto.AccountType);
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID.ToString(),
            AccountType: AccountTypes.Keys.SAVINGS.ToString(),
            Name: "Test Savings Account",
            Description: "Test Savings Account Description",
            InterestRate: 10, DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
            request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID.ToString(), accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.SAVINGS.ToString(), accountDto.AccountType);
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID.ToString(),
            AccountType: AccountTypes.Keys.CREDIT_CARD.ToString(),
            Name: "Test Credit Card Account",
            Description: "Test Credit Card Account Description",
            CreditLimit: 1000,
            InterestRate: 10, DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
            request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID.ToString(), accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.CREDIT_CARD.ToString(), accountDto.AccountType);
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID.ToString(),
            AccountType: AccountTypes.Keys.LINE_OF_CREDIT.ToString(),
            Name: "Test Line Of Credit Account",
            Description: "Test Line Of Credit Account Description",
            CreditLimit: 1000,
            InterestRate: 10, DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
            request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(USER_ID.ToString(), accountDto.UserId);
        Assert.Equal(AccountTypes.Keys.LINE_OF_CREDIT.ToString(), accountDto.AccountType);
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        CreateAccountCommand? request = null;

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
    }

    [Fact]
    public async Task CannotCall_CreateAccount_WithInvalidRequest_EmptyUserId_ReturnsBadRequest()
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: string.Empty,
            AccountType: "Cash",
            Name: "Test Cash Account",
            Description: "Test Cash Account Description", DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID.ToString(),
            AccountType: string.Empty,
            Name: "Test Cash Account",
            Description: "Test Cash Account Description", DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID.ToString(),
            AccountType: AccountTypes.Keys.CASH.ToString(),
            Name: string.Empty,
            Description: "Test Cash Account Description",
            DisplayColor: "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID.ToString(),
            AccountType: "CreditCard",
            Name: "Test Cash Account",
            Description: "Test Cash Account Description",
            CreditLimit: -1,
            DisplayColor: "#000000"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID.ToString(),
            AccountType: "CreditCard",
            Name: "Test Cash Account",
            Description: "Test Cash Account Description",
            CreditLimit: 1000,
            InterestRate: -1,
            DisplayColor: "#000000"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
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
        var USER_ID = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateAccountCommand
        (
            UserId: USER_ID.ToString(),
            AccountType: "CreditCard",
            Name: "Test Cash Account",
            Description: "Test Cash Account Description",
            CreditLimit: 1000,
            InterestRate: 101,
            DisplayColor: "#000000"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", USER_ID.ToString()),
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