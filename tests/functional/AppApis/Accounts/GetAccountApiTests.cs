using System.Text.Json;
using Habanerio.Xpnss.Shared.DTOs;

using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Accounts;

public class GetAccountApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    : BaseFunctionalApisTests(factory),
        IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
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
                ENDPOINTS_ACCOUNTS_GET_ACCOUNT
                    .Replace("{userId}", USER_ID.ToString())
                    .Replace("{accountId}", availableAccount.Id.ToString()));

            // Assert
            //response.EnsureSuccessStatusCode();
            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);

            var apiResponse = JsonSerializer.Deserialize<AccountDto>(
                content,
                JsonSerializationOptions);

            Assert.NotNull(apiResponse);

            var accountDto = Assert.IsType<AccountDto>(apiResponse);

            Assert.NotNull(accountDto);
            Assert.Equal(USER_ID.ToString(), accountDto.UserId);
            Assert.Equal(availableAccount.Id.ToString(), accountDto.Id);
            Assert.Equal(availableAccount.AccountType, accountDto.AccountType);
            Assert.Equal(availableAccount.BankAccountType, accountDto.BankAccountType);
            Assert.Equal(availableAccount.LoanAccountType, accountDto.LoanAccountType);

            Assert.Equal(availableAccount.ClosedDate, accountDto.ClosedDate);

            Assert.Equal(availableAccount.IsCredit, accountDto.IsCredit);
            Assert.Equal(availableAccount.IsDeleted, accountDto.IsDeleted);

            Assert.Equal(availableAccount.Balance, accountDto.Balance);
            Assert.Equal(availableAccount.CreditLimit, accountDto.CreditLimit);
            Assert.Equal(availableAccount.InterestRate, accountDto.InterestRate);
            Assert.Equal(availableAccount.OverdraftLimit, accountDto.OverdraftLimit);


            Assert.Equal(availableAccount.DateCreated, accountDto.DateCreated);
            Assert.Equal(availableAccount.DateUpdated, accountDto.DateUpdated);
            Assert.Equal(availableAccount.DateDeleted, accountDto.DateDeleted);

            //if (availableAccount.AccountType is AccountEnums.CurrencyKeys.CASH cashAccountDoc)
            //{
            //    Assert.NotNull(cashAccountDoc);
            //}
            //else if (availableAccount is CheckingAccountDocument checkingAccountDoc)
            //{
            //    Assert.NotNull(checkingAccountDoc);
            //    Assert.Equal(checkingAccountDoc.OverdraftLimit, accountDto.OverdraftLimit);
            //}
            //else if (availableAccount is SavingsAccountDocument savingsAccountDoc)
            //{
            //    Assert.NotNull(savingsAccountDoc);
            //    Assert.Equal(savingsAccountDoc.InterestRate, accountDto.InterestRate);
            //}
            //else if (availableAccount is CreditCardAccountDocument ccAccountDoc)
            //{
            //    Assert.NotNull(ccAccountDoc);
            //    Assert.Equal(ccAccountDoc.CreditLimit, accountDto.CreditLimit);
            //    Assert.Equal(ccAccountDoc.InterestRate, accountDto.InterestRate);
            //}
            //else if (availableAccount is LineOfCreditAccountDocument lineOfCreditAccountDoc)
            //{
            //    Assert.NotNull(lineOfCreditAccountDoc);
            //    Assert.Equal(lineOfCreditAccountDoc.CreditLimit, accountDto.CreditLimit);
            //    Assert.Equal(lineOfCreditAccountDoc.InterestRate, accountDto.InterestRate);
            //}
        }
    }
}