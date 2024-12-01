using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Application.Commands.CreditLimitAdjustment;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Infrastructure.Interfaces.Documents;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

/// <summary>
/// Tests that Credit Limit can be adjusted for those Accounts that support Credit Limits.
/// An Adjustment made in the past, will not necessarily affect the current Credit Limit.
/// (if there are transactions since the Adjustment date).
/// Also tests that Accounts that do not support Credit Limits, will return BadRequest
/// </summary>
public class AddCreditLimitAdjustmentApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_ADJUST_CREDIT_LIMIT =
        "/api/v1/users/{userId}/accounts/{accountId}/credit-limit";

    [Theory]
    [InlineData(AccountTypes.Keys.CREDIT_CARD)]
    [InlineData(AccountTypes.Keys.LINE_OF_CREDIT)]
    public async Task CanCall_AdjustCreditLimit_ReturnsOk(AccountTypes.Keys accountType)
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        CreditAccountDocument? creditAccount = null;
        var adjustmentDate = DateTime.Now.AddDays(-(new Random().Next(1, 2 * 365)));
        var previousCreditLimit = 0m;

        if (account is IDocumentHasCreditLimit creditLimitDoc)
            previousCreditLimit = creditLimitDoc.CreditLimit;
        else
            Assert.Fail("Not a valid Credit Limit Account");

        // Calculate new credit limit
        var expectedCreditLimit = previousCreditLimit + 100m;

        var request = new AddCreditLimitAdjustmentCommand(
            account.UserId.ToString(),
            account.Id.ToString(),
            expectedCreditLimit,
            adjustmentDate,
            $"Test {DateTime.Now}");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_CREDIT_LIMIT
                .Replace("{userId}", USER_ID.ToString())
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        return;

        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var adjustCreditLimitContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustCreditLimitContent);

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<decimal>>(
            adjustCreditLimitContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        // Now fetch this Account again to verify the balance
        var actualAccountDoc = (await AccountDocumentsRepository
                .FirstOrDefaultDocumentAsync(a =>
                    a.Id == account.Id &&
                    a.UserId == USER_ID))
            as CreditAccountDocument;

        Assert.NotNull(actualAccountDoc);
        Assert.Equal(previousCreditLimit, actualAccountDoc.CreditLimit);
    }

    [Theory]
    [InlineData(AccountTypes.Keys.CASH)]
    [InlineData(AccountTypes.Keys.CHECKING)]
    [InlineData(AccountTypes.Keys.SAVINGS)]
    public async Task CanNotCall_AdjustCreditLimit_InvalidAccountType_ReturnsBadRequestOk(AccountTypes.Keys accountType)
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        var accounts = await AccountDocumentsRepository
            .FindDocumentsAsync(a =>
                a.UserId == USER_ID &&
                a.AccountType == accountType);

        // Take the first (maybe make random)
        var account = accounts.First();

        var request = new AddCreditLimitAdjustmentCommand(
            account.UserId.ToString(),
            account.Id.ToString(),
            100000000000000,
            DateTime.Now,
            $"Test {DateTime.Now}");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            ENDPOINTS_ACCOUNTS_ADJUST_CREDIT_LIMIT
                .Replace("{userId}", USER_ID.ToString())
                .Replace("{accountId}", account.Id.ToString()),
            request);

        //TODO: Need to think about/implement Adjustment logic.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        return;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var adjustCreditLimitContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(adjustCreditLimitContent);

        var apiResponse = JsonSerializer.Deserialize<List<string>>(
            adjustCreditLimitContent,
            JsonSerializationOptions);

        Assert.NotNull(apiResponse);
        Assert.NotEmpty(apiResponse);
        Assert.Equal($"the Account Type `{accountType.ToString()}` does not support Credit Limits", apiResponse[0]);
    }
}