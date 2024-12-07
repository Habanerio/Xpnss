using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Application.Commands.UpdateAccountDetails;
using Habanerio.Xpnss.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class UpdateAccountDetailsApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    [Fact]
    public async Task CanCall_UpdateAccountDetails_ReturnsOk()
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID);
        var account = accounts.First();

        var newName = Guid.NewGuid().ToString();
        var newDescription = Guid.NewGuid().ToString();
        var newDisplayColor = "#f0f0f0";

        // Arrange
        var updateAccountDetailsRequest = new UpdateAccountDetailsCommand(
            account.UserId.ToString(),
            account.Id.ToString(),
            newName,
            newDescription,
            newDisplayColor);

        // Act
        var updateAccountDetailsResponse = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_UPDATE_ACCOUNT_DETAILS
                .Replace("{userId}", USER_ID.ToString())
                .Replace("{accountId}", account.Id.ToString()),
            updateAccountDetailsRequest);

        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(updateAccountDetailsResponse.IsSuccessStatusCode);

        var updateAccountDetailsContent = await updateAccountDetailsResponse.Content.ReadAsStringAsync();
        Assert.NotNull(updateAccountDetailsContent);

        var apiResponse = JsonSerializer.Deserialize<AccountDto>(
            updateAccountDetailsContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);

        var accountDto = Assert.IsType<AccountDto>(apiResponse);

        Assert.NotNull(accountDto);
        Assert.Equal(USER_ID.ToString(), accountDto.UserId);
        Assert.Equal(account.Id.ToString(), accountDto.Id);

        Assert.Equal(newName, accountDto.Name);
        Assert.Equal(newDescription, accountDto.Description);
        Assert.Equal(newDisplayColor, accountDto.DisplayColor);

        Assert.Equal(account.Balance, accountDto.Balance);

        Assert.Equal(account.DateCreated, accountDto.DateCreated);
        Assert.Equal(account.DateDeleted, accountDto.DateDeleted);
        Assert.Equal(account.IsCredit, accountDto.IsCredit);

    }
}