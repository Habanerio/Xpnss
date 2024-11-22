using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Accounts.Domain.Entities;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Accounts.Infrastructure.Mappers;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data.Repositories;

public class AccountMonthlyTotalsRepository(
    IOptions<MongoDbSettings> options,
    IMongoDatabase mongoDb,
    IMediator? mediator = null) :
    MongoDbRepository<AccountMonthlyTotalDocument>(new AccountsDbContext(mongoDb)),
    IAccountMonthlyTotalsRepository
{
    public async Task<Result<AccountMonthlyTotal?>> AddAsync(
        AccountMonthlyTotal accountMonthlyTotal,
        CancellationToken cancellationToken = default)
    {
        var existingMonthlyTotalDoc = await FirstOrDefaultDocumentAsync(a =>
                a.UserId.Equals(accountMonthlyTotal.UserId) &&
                a.AccountId.Equals(ObjectId.Parse(accountMonthlyTotal.AccountId)) &&
                a.Year == accountMonthlyTotal.Year &&
                a.Month == accountMonthlyTotal.Month,
            cancellationToken);

        if (existingMonthlyTotalDoc is not null)
        {
            var updateResult = await UpdateAsync(accountMonthlyTotal, cancellationToken);

            return updateResult;
        }

        var monthlyTotalDoc = Mapper.Map(accountMonthlyTotal);

        if (monthlyTotalDoc is null)
            return Result.Fail<AccountMonthlyTotal?>("Could not map the AccountMonthlyTotal to its Document");

        await AddDocumentAsync(monthlyTotalDoc, cancellationToken);

        var newMonthlyTotal = Mapper.Map(monthlyTotalDoc);

        return Result.Ok<AccountMonthlyTotal?>(newMonthlyTotal);
    }

    public async Task<Result<AccountMonthlyTotal?>> GetAsync(
        string userId,
        string accountId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(accountId, out var accountObjectId) ||
            accountObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{accountId}`");

        var monthlyTotalDoc = await FirstOrDefaultDocumentAsync(a =>
                a.UserId.Equals(userId) &&
                a.AccountId.Equals(accountObjectId) &&
                a.Year == year &&
                a.Month == month,
            cancellationToken);

        if (monthlyTotalDoc is null)
            return Result.Ok<AccountMonthlyTotal?>(null);

        var accountMonthlyTotal = Mapper.Map(monthlyTotalDoc);

        if (accountMonthlyTotal is null)
            return Result.Fail<AccountMonthlyTotal?>("Could not map the AccountMonthlyTotal Document to its Entity");

        return accountMonthlyTotal;
    }

    public Task<IEnumerable<AccountMonthlyTotal>> ListAsync(string userId, string accountId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AccountMonthlyTotal>> ListAsync(string userId, string accountId, int year, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AccountMonthlyTotal>> ListAsync(string userId, string accountId, int year, int month, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }



    public async Task<Result<AccountMonthlyTotal>> UpdateAsync(
        AccountMonthlyTotal accountMonthlyTotal,
        CancellationToken cancellationToken = default)
    {
        var monthlyTotalDoc = Mapper.Map(accountMonthlyTotal);

        if (monthlyTotalDoc is null)
            return Result.Fail<AccountMonthlyTotal>("Could not map the AccountMonthlyTotal to its Document");

        var updateResult = await UpdateDocumentAsync(monthlyTotalDoc, cancellationToken);

        if (updateResult > 0)
            return Result.Ok(accountMonthlyTotal);

        return Result.Fail<AccountMonthlyTotal>("Could not update the AccountMonthlyTotal");
    }
}