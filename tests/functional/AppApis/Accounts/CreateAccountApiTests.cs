using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Habanerio.Xpnss.Accounts.Application.Commands.CreateAccount;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.Types;

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
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateCashAccountRequest(
            userId.ToString(),
            "Test Cash Account",
            "Test Cash Account Description",
            "#123ABC");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT
                .Replace("{userId}", userId.ToString()),
            request);

        // Assert
        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);

                Assert.Fail(string.Join(Environment.NewLine, errors));
            }

        var apiResponse = JsonSerializer.Deserialize<AccountDto>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);

        var accountDto = Assert.IsType<AccountDto>(apiResponse);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(userId.ToString(), accountDto.UserId);
        Assert.Equal(AccountEnums.AccountKeys.CASH, accountDto.AccountType);
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
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateCheckingAccountRequest(
            userId.ToString(),
            "Test Checking Account",
            "Test Checking Account Description",
            "#0df000",
            4232);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
            request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<AccountDto>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);

        var accountDto = Assert.IsType<AccountDto>(apiResponse);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(userId.ToString(), accountDto.UserId);
        Assert.Equal(AccountEnums.AccountKeys.BANK, accountDto.AccountType);
        //Assert.Equal(BankAccountEnums.CurrencyKeys.CHECKING, accountDto.);
        Assert.Equal(request.Name, accountDto.Name);
        Assert.Equal(0, accountDto.Balance);
        Assert.Equal(request.Description, accountDto.Description);
        Assert.Equal(request.DisplayColor, accountDto.DisplayColor);
        Assert.Equal(request.OverdraftAmount, accountDto.OverdraftLimit);
        Assert.Equal(DateTime.UtcNow, accountDto.DateCreated, TimeSpan.FromSeconds(10));
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
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateSavingsAccountRequest(
             userId.ToString(),
             "Test Savings Account",
             "Test Savings Account Description",
             "#000000",
             10);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
            request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<AccountDto>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);

        var accountDto = Assert.IsType<AccountDto>(apiResponse);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(userId.ToString(), accountDto.UserId);
        Assert.Equal(AccountEnums.AccountKeys.BANK, accountDto.AccountType);
        //BANK ACCOUNT ENUM = SAVINGS
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
    public async Task CanCall_CreateAccount_CreateCreditLineAccount_WithValidRequest_ReturnsOk()
    {
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateCreditLineAccountRequest(
            userId.ToString(),
            "Test Line Of Credit Account",
            "Test Line Of Credit Account Description",
            "#000000",
            1000,
            10);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
            request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<AccountDto>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);

        var accountDto = Assert.IsType<AccountDto>(apiResponse);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(userId.ToString(), accountDto.UserId);
        Assert.Equal(AccountEnums.AccountKeys.BANK, accountDto.AccountType);
        // BANK ACCOUNT ENUM = CREDIT_LINE
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
    public async Task CanCall_CreateAccount_CreditCardAccount_WithValidRequest_ReturnsOk()
    {
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateCreditCardAccountRequest(
             userId.ToString(),
             "Test Credit Card Account",
             "Test Credit Card Account Description",
             "#000000",
             1000,
            10);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
            request);

        // Assert
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<AccountDto>(content, JsonSerializationOptions);

        Assert.NotNull(apiResponse);

        var accountDto = Assert.IsType<AccountDto>(apiResponse);
        Assert.NotEqual(accountDto.Id, ObjectId.Empty.ToString());
        Assert.Equal(userId.ToString(), accountDto.UserId);
        Assert.Equal(AccountEnums.AccountKeys.CREDITCARD, accountDto.AccountType);
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
    public async Task CannotCall_CreateAccount_WithInvalidRequest_NULL_ReturnsBadRequest()
    {
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        CreateAccountCommand? request = null;

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
    }


    //[Fact]
    //public async Task CannotCall_CreateAccount_WithInvalidRequest_EmptyAccountType_ReturnsBadRequest()
    //{
    //    var userId = await GetTestUserObjectIdAsync();

    //    // Arrange
    //    var apiRequest = new CreateAccountCommand
    //    (
    //        UserId: userId.ToString(),
    //        AccountType: string.Empty,
    //        Name: "Test Cash Account",
    //        Description: "Test Cash Account Description", DisplayColor: "#000000");

    //    // Act
    //    var response = await HttpClient.PostAsJsonAsync(
    //        ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
    //        apiRequest);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    //    var content = await response.Content.ReadAsStringAsync();

    //    var errors = JsonSerializer.Deserialize<List<string>>(content, JsonSerializationOptions);

    //    Assert.NotNull(errors);
    //    Assert.Single(errors);
    //    Assert.Equal("'Account Type' must not be empty.", errors[0]);
    //}

    [Fact]
    public async Task CannotCall_CreateAccount_WithInvalidRequest_EmptyName_ReturnsBadRequest()
    {
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateCashAccountRequest(
            userId.ToString(),
            string.Empty,
            "Test Cash Account Description",
            "#000000");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
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
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateCreditCardAccountRequest(
            userId.ToString(),
        "Test Cash Account",
    "Test Cash Account Description",
            "#000000",
            -1,
             10);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
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
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateCreditCardAccountRequest(
            userId.ToString(),
            "Test Cash Account",
            "Test Cash Account Description",
            "#000000",
            1000,
            -1);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
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
        var userId = await GetTestUserObjectIdAsync();

        // Arrange
        var request = new CreateCreditCardAccountRequest(
            userId.ToString(),
            "Test Cash Account",
            "Test Cash Account Description",
            "#000000",
            1000,
            101);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT.Replace("{userId}", userId.ToString()),
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