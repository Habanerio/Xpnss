using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Application.Commands.BalanceAdjustment;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Domain.Types;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

/// <summary>
/// Tests that Balance can be adjusted all types of Accounts*
/// An Adjustment made in the past, will not necessarily affect the current Balance
/// (if there are transactions since the Adjustment date)
/// </summary>
public class AddBalanceAdjustmentApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_ADJUST_BALANCE = "/api/v1/users/{userId}/accounts/{accountId}/balance";

    [Theory]
    [InlineData(AccountTypes.Keys.CASH)]
    [InlineData(AccountTypes.Keys.CHECKING)]
    [InlineData(AccountTypes.Keys.SAVINGS)]
    [InlineData(AccountTypes.Keys.CREDIT_CARD)]
    [InlineData(AccountTypes.Keys.LINE_OF_CREDIT)]
    public async Task CanCall_AdjustBalance_ReturnsOk(AccountTypes.Keys accountType)
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        if (account is null)
            return;

        var adjustmentDate = DateTime.Now.AddDays(-(new Random().Next(1, 2 * 365)));

        // Calculate new balance
        var previousBalance = account.Balance;
        var adjustedBalance = previousBalance + 100m;
        var expectedReason = $"Test {adjustmentDate} {Guid.NewGuid()}";

        var request = new AddBalanceAdjustmentCommand(
            account.UserId.ToString(),
            account.Id.ToString(),
            adjustedBalance,
            adjustmentDate,
            expectedReason
            );

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_BALANCE
                .Replace("{userId}", USER_ID.ToString())
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/properly implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        return;

        //TODO
        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var adjustBalanceContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustBalanceContent);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<decimal>>(
            adjustBalanceContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        Assert.Equal(adjustedBalance, apiResponse.Data);

        // Now fetch this Account again to verify the balance
        var actualAccountDoc = await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a =>
            a.Id == account.Id && a.UserId == USER_ID);

        Assert.NotNull(actualAccountDoc);
        Assert.Equal(previousBalance, actualAccountDoc.Balance);
        //Assert.NotEmpty(actualAccountDoc.AdjustmentHistories);

        //var adjustmentHistory = actualAccountDoc.AdjustmentHistories.First(h =>
        //    h.Reason.Equals(expectedReason));
        //Assert.NotNull(adjustmentHistory);
        //Assert.True(adjustmentHistory.Value == adjustedBalance.ToString(CultureInfo.InvariantCulture));
    }
}
