using System.Net;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class GetAccountApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_ACCOUNTS_GET_ACCOUNTS = "/api/v1/users/{userId}/accounts/{accountId}";

    [Fact]
    public async Task GetAccount_ReturnsOk()
    {
        var USER_ID = await GetTestUserObjectIdAsync();

        var availableAccounts =
            (await AccountDocumentsRepository.FindDocumentsAsync(a =>
                a.UserId == USER_ID)).ToList();

        foreach (var availableAccount in availableAccounts)
        {
            // Act
            var response = await HttpClient.GetAsync(
                ENDPOINTS_ACCOUNTS_GET_ACCOUNTS
                    .Replace("{userId}", USER_ID.ToString())
                    .Replace("{accountId}", availableAccount.Id.ToString()));

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountDto>>(
                content,
                JsonSerializationOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.IsSuccess);

            var accountDto = Assert.IsType<AccountDto>(apiResponse.Data);
            Assert.NotNull(accountDto);
            Assert.Equal(USER_ID.ToString(), accountDto.UserId);
            Assert.Equal(availableAccount.Id.ToString(), accountDto.Id);
            Assert.Equal(availableAccount.AccountType.ToString(), accountDto.AccountType);
            Assert.Equal(availableAccount.Balance, accountDto.Balance);
            Assert.Equal(availableAccount.DateCreated, accountDto.DateCreated);
            Assert.Equal(availableAccount.IsCredit, accountDto.IsCredit);
            Assert.Equal(availableAccount.DateDeleted, accountDto.DateDeleted);

            if (availableAccount is CashAccountDocument cashAccountDoc)
            {
                Assert.NotNull(cashAccountDoc);
            }
            else if (availableAccount is CheckingAccountDocument checkingAccountDoc)
            {
                Assert.NotNull(checkingAccountDoc);
                Assert.Equal(checkingAccountDoc.OverdraftAmount, accountDto.OverdraftAmount);
            }
            else if (availableAccount is SavingsAccountDocument savingsAccountDoc)
            {
                Assert.NotNull(savingsAccountDoc);
                Assert.Equal(savingsAccountDoc.InterestRate, accountDto.InterestRate);
            }
            else if (availableAccount is CreditCardAccountDocument ccAccountDoc)
            {
                Assert.NotNull(ccAccountDoc);
                Assert.Equal(ccAccountDoc.CreditLimit, accountDto.CreditLimit);
                Assert.Equal(ccAccountDoc.InterestRate, accountDto.InterestRate);
            }
            else if (availableAccount is LineOfCreditAccountDocument lineOfCreditAccountDoc)
            {
                Assert.NotNull(lineOfCreditAccountDoc);
                Assert.Equal(lineOfCreditAccountDoc.CreditLimit, accountDto.CreditLimit);
                Assert.Equal(lineOfCreditAccountDoc.InterestRate, accountDto.InterestRate);
            }
        }
    }
}