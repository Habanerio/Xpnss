using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.Accounts.Commands.UpdateAccountDetails;
using Habanerio.Xpnss.Application.Accounts.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class UpdateAccountDetailsApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_UPDATE_ACCOUNT_DETAILS = "/api/v1/users/{userId}/accounts/{accountId}";

    [Fact]
    public async Task CanCall_UpdateAccountDetails_ReturnsOk()
    {
        var accounts = await AccountDocumentsRepository.FindAsync(a => a.UserId == USER_ID);
        var account = accounts.First();

        var newName = Guid.NewGuid().ToString();
        var newDescription = Guid.NewGuid().ToString();
        var newDisplayColor = "#f0f0f0";

        // Arrange
        var updateAccountDetailsRequest = new UpdateAccountDetailsCommand(
            account.UserId,
            account.Id.ToString(),
            newName,
            newDescription,
            newDisplayColor);

        // Act
        var updateAccountDetailsResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_UPDATE_ACCOUNT_DETAILS
                .Replace("{userId}", USER_ID)
                .Replace("{accountId}", account.Id.ToString()),
            updateAccountDetailsRequest);

        updateAccountDetailsResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, updateAccountDetailsResponse.StatusCode);

        var updateAccountDetailsContent = await updateAccountDetailsResponse.Content.ReadAsStringAsync();
        Assert.NotNull(updateAccountDetailsContent);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(
            updateAccountDetailsContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
        Assert.NotNull(accountDto);
        Assert.Equal(USER_ID, accountDto.UserId);
        Assert.Equal(account.Id.ToString(), accountDto.Id);
        Assert.Equal(newName, accountDto.Name);
        Assert.Equal(newDescription, accountDto.Description);
        Assert.Equal(account.Balance, accountDto.Balance);
        Assert.Equal(account.DisplayColor, accountDto.DisplayColor);
        Assert.Equal(account.DateCreated, accountDto.DateCreated);
        Assert.Equal(account.DateDeleted, accountDto.DateDeleted);
        Assert.Equal(account.IsCredit, accountDto.IsCredit);

    }
}