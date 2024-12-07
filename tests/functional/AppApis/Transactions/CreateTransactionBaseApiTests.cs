using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Documents;
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
    /// <param name="originalRequest"></param>
    /// <param name="transactionType">The type of Transaction that the Asserts should be testing</param>
    /// <returns></returns>
    protected async Task AssertTransactionAsync(
        ObjectId testUserId,
        AccountDocument? originalAccountDoc,
        CreateTransactionRequest originalRequest,
        TransactionEnums.TransactionKeys transactionType)
    {
        Assert.NotNull(originalRequest);

        Assert.True(
            (!string.IsNullOrWhiteSpace(originalRequest.AccountId) && originalAccountDoc is not null) ||
            (string.IsNullOrWhiteSpace(originalRequest.AccountId) && originalAccountDoc is null));

        var originalMonthlyTotalDocs = (await GetMonthlyTotalsAsync(
            testUserId,
            originalRequest.TransactionDate.Year,
            originalRequest.TransactionDate.Month)).ToList();

        TransactionDto actualTransactionDto;

        if (transactionType is TransactionEnums.TransactionKeys.DEPOSIT)
        {
            var createDepositTransactionRequest = originalRequest as CreateDepositTransactionRequest ??
                throw new InvalidOperationException(nameof(originalRequest));

            Assert.NotNull(createDepositTransactionRequest);

            actualTransactionDto =
                await GetCreateTransactionFromApi<CreateDepositTransactionRequest, DepositTransactionDto>
                    (testUserId, createDepositTransactionRequest);

            await AssertTransactionResultDtoAsync(
                testUserId,
                createDepositTransactionRequest,
                originalAccountDoc,
                originalMonthlyTotalDocs,
                actualTransactionDto);
        }
        else if (transactionType is TransactionEnums.TransactionKeys.PURCHASE)
        {
            var createPurchaseTransactionRequest = originalRequest as CreatePurchaseTransactionRequest ??
                throw new InvalidOperationException(nameof(originalRequest));

            Assert.NotNull(createPurchaseTransactionRequest);

            actualTransactionDto =
                await GetCreateTransactionFromApi<CreatePurchaseTransactionRequest, PurchaseTransactionDto>
                    (testUserId, createPurchaseTransactionRequest);

            await AssertTransactionResultDtoAsync(
                testUserId,
                createPurchaseTransactionRequest,
                originalAccountDoc,
                originalMonthlyTotalDocs,
                actualTransactionDto);

            Assert.NotNull(actualTransactionDto);
        }
        else
        {
            Assert.Fail($"'{transactionType}' is an unknown type");
        }

        //// Check that the Account Document's Balance has been updated after the request was made
        //AccountDocument? updatedAccountDoc =
        //    await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a =>
        //    a.Id == originalAccountDoc.Id && a.UserId == testUserId);

        //var updatedMonthlyTotalDocs = (await GetMonthlyTotalsAsync(
        //    testUserId,
        //    originalRequest.TransactionDate.Year,
        //    originalRequest.TransactionDate.Month)).ToList();

        //Assert.NotNull(actualTransactionDto);
        //Assert.True(!actualTransactionDto.Id.Equals(ObjectId.Empty.ToString()));
        //Assert.Equal(originalRequest.UserId, actualTransactionDto.UserId);

        //Assert.Equal(originalRequest.Description, actualTransactionDto.Description);

        //// If a Debit Request, then dto.IsCredit should be false
        //Assert.True(actualDepositTransactionDto is null || !actualDepositTransactionDto.IsCredit);

        //// If a Credit Request, then dto.IsCredit should be true
        //Assert.True(actualPurchaseTransactionDto is null || actualPurchaseTransactionDto.IsCredit);

        //Assert.Equal(originalRequest.TransactionDate.Date, actualTransactionDto.TransactionDate.Date);

        //Assert.Equal(transactionType, actualTransactionDto.TransactionType);

        //AssertPayerPayee(originalRequest, actualTransactionDto);

        //if (actualPurchaseTransactionDto is not null)
        //{
        //    Assert.NotNull(actualPurchaseTransactionDto);

        //    Assert.Equal(createPurchaseTransactionRequest.Items.Count, actualPurchaseTransactionDto.Items.Count);


        //    Assert.Equal(createPurchaseTransactionRequest.Items.Sum(i =>
        //        i.Amount), actualPurchaseTransactionDto.TotalAmount);

        //    // New transaction, Total Owing should be the same as Total Amount
        //    Assert.Equal(createPurchaseTransactionRequest.Items.Sum(i =>
        //        i.Amount), actualPurchaseTransactionDto.TotalOwing);

        //    // New transaction, there should be no payments
        //    Assert.Equal(0, actualPurchaseTransactionDto.TotalPaid);

        //    foreach (var item in createPurchaseTransactionRequest.Items)
        //    {
        //        var actualTransactionItem = actualPurchaseTransactionDto.Items.Find(i =>
        //            i.CategoryId == item.CategoryId &&
        //            i.Description == item.Description &&
        //            i.Amount == item.Amount);

        //        Assert.NotNull(actualTransactionItem);
        //    }
        //}

        //AssertAccount(testUserId, originalAccountDoc, updatedAccountDoc, actualTransactionDto);

        //// Monthly Totals
        //Assert.NotNull(updatedMonthlyTotalDocs);
        //Assert.NotEmpty(updatedMonthlyTotalDocs);

        //var originalAccountMonthlyTotalDoc = originalMonthlyTotalDocs.Find(t =>
        //    t.Year == originalRequest.TransactionDate.Year &&
        //    t.Month == originalRequest.TransactionDate.Month);

        //if (originalAccountMonthlyTotalDoc is not null)
        //{
        //    var updatedAccountMonthlyTotalDoc = updatedMonthlyTotalDocs.Find(t =>
        //        t.Year == originalRequest.TransactionDate.Year &&
        //        t.Month == originalRequest.TransactionDate.Month);

        //    Assert.NotNull(updatedAccountMonthlyTotalDoc);

        //    // TODO: Finish this ...
        //}

        ////if (actualPurchaseTransactionDto is not null)
        ////{
        ////    var monthlyTotalDoc = updatedMonthlyTotalDocs.Find(t =>
        ////        t.Year == originalRequest.TransactionDate.Year &&
        ////        t.Month == originalRequest.TransactionDate.Month);
        ////}

        ////Assert.NotEmpty(actualAccount.MonthlyTotals);

        ////Assert.NotNull(actualAccount.MonthlyTotals.Find(t =>
        ////    t.Year == transactionDate.Year && t.Month == transactionDate.Month));
    }

    private async Task AssertTransactionResultDtoAsync<TRequest, TDto>(
        ObjectId testUserId,
        TRequest createTransactionRequest,
        AccountDocument? originalAccountDoc,
        List<MonthlyTotalDocument>? originalMonthlyTotalDocs,
        TDto actualTransactionDto)
        where TRequest : CreateTransactionRequest where TDto : TransactionDto
    {
        Assert.NotNull(actualTransactionDto);

        // If there was an original Account Document, then there should be an updated Account Document.
        // If there was an original Account Document, and there is NO updated Account Document, then throw an exception.
        // Else, then there should be no updated Account Document (null).
        var updatedAccountDoc = (
            originalAccountDoc is not null ?
                (await AccountDocumentsRepository.FirstOrDefaultDocumentAsync(a =>
                    a.Id == originalAccountDoc.Id && a.UserId == testUserId
                ) ?? throw new InvalidOperationException("Could not find the updatedAccountDoc"))
            : null);

        var updatedMonthlyTotalDocs = (await GetMonthlyTotalsAsync(
            testUserId,
            createTransactionRequest.TransactionDate.Year,
            createTransactionRequest.TransactionDate.Month)).ToList();

        Assert.Equal(createTransactionRequest.UserId, actualTransactionDto.UserId);
        Assert.Equal(createTransactionRequest.AccountId, actualTransactionDto.AccountId);
        Assert.Equal(createTransactionRequest.Description, actualTransactionDto.Description);
        Assert.Equal(createTransactionRequest.ExtTransactionId, actualTransactionDto.ExtTransactionId);
        Assert.Equal(createTransactionRequest.IsCredit, actualTransactionDto.IsCredit);
        Assert.Equal(createTransactionRequest.TransactionDate.Date, actualTransactionDto.TransactionDate);
        Assert.Equal(createTransactionRequest.Tags, actualTransactionDto.Tags);
        Assert.Equal(createTransactionRequest.TotalAmount, actualTransactionDto.TotalAmount);
        Assert.Equal(createTransactionRequest.TransactionType, actualTransactionDto.TransactionType);

        AssertAccount(testUserId,
            createTransactionRequest,
            originalAccountDoc,
            updatedAccountDoc,
            actualTransactionDto);

        if (createTransactionRequest is
                CreateDepositTransactionRequest depositRequest &&
            actualTransactionDto is
                DepositTransactionDto depositDto)
        {
            AssertDepositTransaction(depositRequest, depositDto);
        }
        else if (createTransactionRequest is
                     CreatePurchaseTransactionRequest purchaseRequest &&
                 actualTransactionDto is
                     PurchaseTransactionDto purchaseDto)
        {
            AssertPurchaseTransaction(purchaseRequest, purchaseDto);
        }
        else
        {
            Assert.Fail(
                $"{nameof(createTransactionRequest)}: " +
                $"'{createTransactionRequest.GetType()}' OR " +
                $"{nameof(actualTransactionDto)}: " +
                $"'{actualTransactionDto}' is of an unknown Type");
        }

        AssertPayerPayee(createTransactionRequest, actualTransactionDto);

        // Monthly Totals
        Assert.NotNull(updatedMonthlyTotalDocs);
        Assert.NotEmpty(updatedMonthlyTotalDocs);

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
        CreateTransactionRequest transactionRequest,
        AccountDocument? originalAccountDoc,
        AccountDocument? updatedAccountDoc,
        TransactionDto actualTransactionDto)
    {
        var transactionType = actualTransactionDto.TransactionType;

        // If an accountBase was expected to be used, then verify that the ExtAcctId matches the request
        // If no accountBase was expected to be used, then verify that the ExtAcctId is null
        if (string.IsNullOrWhiteSpace(transactionRequest.AccountId) &&
            originalAccountDoc is null)
        {
            Assert.True(string.IsNullOrWhiteSpace(actualTransactionDto.AccountId));
        }
        else if (!string.IsNullOrWhiteSpace(transactionRequest.AccountId) &&
                originalAccountDoc is not null)
        {
            var previousAccountBalance = originalAccountDoc.Balance;

            Assert.True(!string.IsNullOrWhiteSpace(actualTransactionDto.AccountId));
            Assert.Equal(originalAccountDoc.Id.ToString(), actualTransactionDto.AccountId);

            if (TransactionEnums.DoesBalanceIncrease(originalAccountDoc.IsCredit, transactionType))
                Assert.Equal(previousAccountBalance + actualTransactionDto.TotalAmount,
                    updatedAccountDoc!.Balance);
            else
                Assert.Equal(previousAccountBalance - actualTransactionDto.TotalAmount,
                    updatedAccountDoc!.Balance);
        }
        else
        {
            Assert.Fail($"Not sure how we got here:" +
                        $"{Environment.NewLine} {nameof(transactionRequest.AccountId)}'s value = '{transactionRequest.AccountId}'" +
                        $"{Environment.NewLine} {nameof(originalAccountDoc)} is null = {originalAccountDoc is null}");
        }
    }

    protected static void AssertDepositTransaction(
        CreateDepositTransactionRequest transactionRequest,
        DepositTransactionDto? transactionDto)
    {
        Assert.NotNull(transactionDto);
        Assert.Equal(transactionRequest.TransactionDate, transactionDto.TransactionDate);
        Assert.Equal(transactionRequest.TotalAmount, transactionDto.TotalAmount);
    }

    protected static void AssertPurchaseTransaction(
        CreatePurchaseTransactionRequest transactionRequest,
        PurchaseTransactionDto? transactionDto)
    {
        Assert.NotNull(transactionDto);
        Assert.Equal(transactionRequest.TransactionDate, transactionDto.TransactionDate);
        Assert.Equal(transactionRequest.TotalAmount, transactionDto.TotalAmount);

        Assert.Equal(transactionRequest.Items.Count, transactionRequest.Items.Count);

        Assert.Equal(transactionRequest.Items.Sum(i =>
            i.Amount), transactionDto.TotalAmount);

        // New transaction, Total Owing should be the same as Total Amount
        Assert.Equal(transactionRequest.Items.Sum(i =>
            i.Amount), transactionDto.TotalOwing);

        // New transaction, there should be no payments
        Assert.False(transactionDto.IsPaid);
        Assert.Null(transactionDto.PaidDate);
        Assert.Equal(0, transactionDto.TotalPaid);

        foreach (var item in transactionRequest.Items)
        {
            var actualTransactionItem = transactionDto.Items.Find(i =>
                i.CategoryId == item.CategoryId &&
                i.Description == item.Description &&
                i.Amount == item.Amount);

            Assert.NotNull(actualTransactionItem);
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

        // If the request has a PayerPayee Id OR a Name,
        // then it must return a PayerPayee Id (whether existing or new)
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
    /// Calls the Create Transaction endpoint and returns a TransactionDto
    /// </summary>
    /// <param name="userId">The Id of the User to create the Transaction for</param>
    /// <param name="createTransactionRequest">The Request to send to the endpoint</param>
    /// <returns>TransactionDto</returns>
    private async Task<TDto> GetCreateTransactionFromApi<TRequest, TDto>(
        ObjectId userId,
        TRequest createTransactionRequest) where TDto : TransactionDto where TRequest : CreateTransactionRequest
    {
        // Act
        var createTransactionResponse = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_TRANSACTIONS_CREATE_TRANSACTION
                .Replace("{userId}", userId.ToString()),
            createTransactionRequest);

        var transactionContent = await createTransactionResponse.Content.ReadAsStringAsync();

        if (!createTransactionResponse.IsSuccessStatusCode)
            Assert.Fail(transactionContent);

        var transactionApiResponse = JsonSerializer.Deserialize<TDto>(
            transactionContent, JsonSerializationOptions);

        // Assert
        Assert.NotNull(transactionApiResponse);

        return Assert.IsAssignableFrom<TDto>(transactionApiResponse);
    }
}