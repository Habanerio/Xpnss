using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.Types;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Transactions;

public class CreateTransactionBaseApiTests(WebApplicationFactory<Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// Helper for Asserting the tests
    /// </summary>
    /// <param name="testUserId">THe ObjectId of the User who is requesting the Transaction</param>
    /// <param name="originalAccountDoc"></param>
    /// <param name="createTransactionRequest"></param>
    /// <param name="transactionType">The type of Transaction that the Asserts should be testing</param>
    /// <returns></returns>
    protected async Task AssertTransactionAsync(
        ObjectId testUserId,
        AccountDocument? originalAccountDoc,
        CreateTransactionRequest createTransactionRequest,
        TransactionTypes.Keys transactionType)
    {
        var originalMonthlyTotalDocs = (await GetMonthlyTotalsAsync(
            testUserId,
            createTransactionRequest.TransactionDate.Year,
            createTransactionRequest.TransactionDate.Month)).ToList();

        TransactionDto actualTransactionDto;

        CreateDepositTransactionRequest? createDepositTransactionRequest = null;
        CreatePurchaseTransactionRequest? createPurchaseTransactionRequest = null;

        DepositTransactionDto? actualDepositTransactionDto = null;
        PurchaseTransactionDto? actualPurchaseTransactionDto = null;

        switch (transactionType)
        {
            case TransactionTypes.Keys.DEPOSIT:
                createDepositTransactionRequest = createTransactionRequest as CreateDepositTransactionRequest;

                Assert.NotNull(createDepositTransactionRequest);

                actualTransactionDto = await GetDepositTransaction(testUserId, createDepositTransactionRequest);

                actualDepositTransactionDto = actualTransactionDto as DepositTransactionDto;

                Assert.NotNull(actualDepositTransactionDto);

                break;
            case TransactionTypes.Keys.PURCHASE:
                createPurchaseTransactionRequest = createTransactionRequest as CreatePurchaseTransactionRequest;

                Assert.NotNull(createPurchaseTransactionRequest);

                actualTransactionDto = await GetPurchaseTransaction(testUserId, createPurchaseTransactionRequest);

                actualPurchaseTransactionDto = actualTransactionDto as PurchaseTransactionDto;

                Assert.NotNull(actualTransactionDto);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null);
        }

        // Check that the Account Document's Balance has been updated after the request was made
        AccountDocument? updatedAccountDoc = originalAccountDoc != null ?
            await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a =>
            a.Id == originalAccountDoc.Id && a.UserId == testUserId) :
            null;

        var updatedMonthlyTotalDocs = (await GetMonthlyTotalsAsync(
            testUserId,
            createTransactionRequest.TransactionDate.Year,
            createTransactionRequest.TransactionDate.Month)).ToList();

        Assert.NotNull(actualTransactionDto);
        Assert.True(!actualTransactionDto.Id.Equals(ObjectId.Empty.ToString()));
        Assert.Equal(createTransactionRequest.UserId, actualTransactionDto.UserId);

        Assert.Equal(createTransactionRequest.Description, actualTransactionDto.Description);

        // If a Debit Request, then dto.IsCredit should be false
        Assert.True(actualDepositTransactionDto is null || !actualDepositTransactionDto.IsCredit);

        // If a Credit Request, then dto.IsCredit should be true
        Assert.True(actualPurchaseTransactionDto is null || actualPurchaseTransactionDto.IsCredit);

        Assert.Equal(createTransactionRequest.TransactionDate.Date, actualTransactionDto.TransactionDate.Date);

        Assert.Equal(transactionType.ToString(), actualTransactionDto.TransactionType);

        AssertPayerPayee(createTransactionRequest, actualTransactionDto);

        if (actualPurchaseTransactionDto is not null && createPurchaseTransactionRequest is not null)
        {
            Assert.NotNull(actualPurchaseTransactionDto);

            Assert.Equal(createPurchaseTransactionRequest.Items.Count, actualPurchaseTransactionDto.Items.Count);


            Assert.Equal(createPurchaseTransactionRequest.Items.Sum(i =>
                i.Amount), actualPurchaseTransactionDto.TotalAmount);

            // New transaction, Total Owing should be the same as Total Amount
            Assert.Equal(createPurchaseTransactionRequest.Items.Sum(i =>
                i.Amount), actualPurchaseTransactionDto.TotalOwing);

            // New transaction, there should be no payments
            Assert.Equal(0, actualPurchaseTransactionDto.TotalPaid);

            foreach (var item in createPurchaseTransactionRequest.Items)
            {
                var actualTransactionItem = actualPurchaseTransactionDto.Items.Find(i =>
                    i.CategoryId == item.CategoryId &&
                    i.Description == item.Description &&
                    i.Amount == item.Amount);

                Assert.NotNull(actualTransactionItem);
            }
        }

        AssertAccount(testUserId, originalAccountDoc, updatedAccountDoc, actualTransactionDto);

        // Monthly Totals
        Assert.NotNull(updatedMonthlyTotalDocs);
        Assert.NotEmpty(updatedMonthlyTotalDocs);

        if (originalAccountDoc is not null)
        {
            var originalAccountMonthlyTotalDoc = originalMonthlyTotalDocs.Find(t =>
                t.Year == createTransactionRequest.TransactionDate.Year &&
                t.Month == createTransactionRequest.TransactionDate.Month);

            if (originalAccountMonthlyTotalDoc is not null)
            {
                var updatedAccountMonthlyTotalDoc = updatedMonthlyTotalDocs.Find(t =>
                    t.Year == createTransactionRequest.TransactionDate.Year &&
                    t.Month == createTransactionRequest.TransactionDate.Month);

                Assert.NotNull(updatedAccountMonthlyTotalDoc);

                // TODO: Finish this ...
            }
        }

        //if (actualPurchaseTransactionDto is not null)
        //{
        //    var monthlyTotalDoc = updatedMonthlyTotalDocs.Find(t =>
        //        t.Year == createTransactionRequest.TransactionDate.Year &&
        //        t.Month == createTransactionRequest.TransactionDate.Month);
        //}

        //Assert.NotEmpty(actualAccount.MonthlyTotals);

        //Assert.NotNull(actualAccount.MonthlyTotals.Find(t =>
        //    t.Year == transactionDate.Year && t.Month == transactionDate.Month));
    }

    protected static void AssertAccount(
        ObjectId testUserId,
        AccountDocument? originalAccountDoc,
        AccountDocument? updatedAccountDoc,
        TransactionDto actualTransactionDto)
    {
        var transactionType = TransactionTypes.ToTransactionType(actualTransactionDto.TransactionType);

        // If an account was expected to be used, then verify that the AccountId matches the request
        // If no account was expected to be used, then verify that the AccountId is null
        if (originalAccountDoc is null)
        {
            Assert.True(string.IsNullOrWhiteSpace(actualTransactionDto.AccountId));
        }
        else
        {
            var previousAccountBalance = originalAccountDoc.Balance;

            Assert.True(!string.IsNullOrWhiteSpace(actualTransactionDto.AccountId));
            Assert.Equal(originalAccountDoc.Id.ToString(), actualTransactionDto.AccountId);



            if (TransactionTypes.DoesBalanceIncrease(originalAccountDoc.AccountType, transactionType))
                Assert.Equal(previousAccountBalance + actualTransactionDto.TotalAmount, updatedAccountDoc!.Balance);
            else
                Assert.Equal(previousAccountBalance - actualTransactionDto.TotalAmount, updatedAccountDoc!.Balance);
        }
    }

    /// <summary>
    /// Asserts the Transaction's PayerPayee details
    /// </summary>
    /// <param name="createTransactionRequest">The request that created the transaction</param>
    /// <param name="actualTransactionDto">The transaction's dto</param>
    protected static void AssertPayerPayee(
        CreateTransactionRequest createTransactionRequest,
        TransactionDto actualTransactionDto)
    {
        // If the request has a PayerPayee Id OR a Name, then it must return a PayerPayee Id (whether existing or new)
        if (!string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Id) ||
            !string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Name))
        {
            Assert.True(!string.IsNullOrWhiteSpace(actualTransactionDto.PayerPayeeId) &&
                        actualTransactionDto.PayerPayeeId != ObjectId.Empty.ToString());

            Assert.NotNull(actualTransactionDto.PayerPayeeId);

            Assert.NotNull(actualTransactionDto.PayerPayee);

            // If the request has a PayerPayee Id, then it must match the response
            if (!string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Id))
                Assert.Equal(createTransactionRequest.PayerPayee.Id,
                    actualTransactionDto.PayerPayeeId);

            if (!string.IsNullOrWhiteSpace(createTransactionRequest.PayerPayee.Name))
            {
                Assert.NotNull(createTransactionRequest.PayerPayee);

                Assert.Equal(createTransactionRequest.PayerPayee.Name,
                    actualTransactionDto.PayerPayee.Name);

                Assert.Equal(createTransactionRequest.PayerPayee.Description,
                    actualTransactionDto.PayerPayee.Description);

                Assert.Equal(createTransactionRequest.PayerPayee.Location,
                    actualTransactionDto.PayerPayee.Location);
            }
        }
    }


    protected static decimal GetRandomAmount(int min, int max) =>
        (decimal)(RandomGenerator.Next(min * 100, max * 100)) / 100;


    /// <summary>
    /// Calls the Create Deposit Transaction endpoint and returns the dto
    /// </summary>
    /// <param name="userId">The Id of the User to create the Transaction for</param>
    /// <param name="createDepositTransactionRequest">The Deposit Request to send to the endpoint</param>
    /// <returns>TransactionDto</returns>
    private async Task<TransactionDto> GetDepositTransaction(
        ObjectId userId,
        CreateDepositTransactionRequest createDepositTransactionRequest)
    {
        // Act
        var createTransactionResponse = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_TRANSACTIONS_CREATE_DEPOSIT_TRANSACTION
                .Replace("{userId}", userId.ToString()),
            createDepositTransactionRequest);

        var transactionContent = await createTransactionResponse.Content.ReadAsStringAsync();

        if (!createTransactionResponse.IsSuccessStatusCode)
            Assert.Fail(transactionContent);

        var transactionApiResponse = JsonSerializer.Deserialize<ApiResponse<DepositTransactionDto>>(
            transactionContent, JsonSerializationOptions);

        // Assert
        Assert.NotNull(transactionApiResponse);
        Assert.True(transactionApiResponse.IsSuccess);

        return Assert.IsAssignableFrom<DepositTransactionDto>(transactionApiResponse.Data);
    }

    /// <summary>
    /// Calls the Create Purchase Transaction endpoint and returns the dto
    /// </summary>
    /// <param name="userId">The Id of the User to create the Transaction for</param>
    /// <param name="createPurchaseTransactionRequest">The Purchase Request to send to the endpoint</param>
    /// <returns>PurchaseTransactionDto</returns>
    private async Task<TransactionDto> GetPurchaseTransaction(
        ObjectId userId,
        CreatePurchaseTransactionRequest createPurchaseTransactionRequest)
    {
        // Act
        var createTransactionResponse = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_TRANSACTIONS_CREATE_PURCHASE_TRANSACTION
                .Replace("{userId}", userId.ToString()),
            createPurchaseTransactionRequest);

        var transactionContent = await createTransactionResponse.Content.ReadAsStringAsync();

        if (!createTransactionResponse.IsSuccessStatusCode)
            Assert.Fail(transactionContent);

        var transactionApiResponse = JsonSerializer.Deserialize<ApiResponse<PurchaseTransactionDto>>(
            transactionContent, JsonSerializationOptions);

        // Assert
        Assert.NotNull(transactionApiResponse);
        Assert.True(transactionApiResponse.IsSuccess);

        return Assert.IsAssignableFrom<PurchaseTransactionDto>(transactionApiResponse.Data);
    }
}