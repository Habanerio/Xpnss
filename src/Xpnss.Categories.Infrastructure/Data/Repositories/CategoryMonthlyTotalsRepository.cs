using FluentResults;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Categories.Domain;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Categories.Infrastructure.Mappers;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Categories.Infrastructure.Data.Repositories;

public class CategoryMonthlyTotalsRepository(
    IOptions<MongoDbSettings> options,
    IMongoDatabase mongoDb,
    IMediator? mediator = null) :
    MongoDbRepository<CategoryMonthlyTotalDocument>(new CategoriesDbContext(mongoDb)),
    ICategoryMonthlyTotalsRepository
{
    public async Task<Result<CategoryMonthlyTotal?>> AddAsync(
        CategoryMonthlyTotal categoryMonthlyTotal,
        CancellationToken cancellationToken = default)
    {
        var existingMonthlyTotalDoc = await FirstOrDefaultDocumentAsync(a =>
                a.UserId.Equals(categoryMonthlyTotal.UserId) &&
                a.CategoryId.Equals(categoryMonthlyTotal.CategoryId) &&
                a.Year == categoryMonthlyTotal.Year &&
                a.Month == categoryMonthlyTotal.Month,
            cancellationToken);

        if (existingMonthlyTotalDoc is not null)
        {
            var updateResult = await UpdateAsync(categoryMonthlyTotal, cancellationToken);

            if (updateResult.IsFailed || updateResult.ValueOrDefault is false)
                return Result.Fail<CategoryMonthlyTotal?>("Could not update the CategoryMonthlyTotal");

            return categoryMonthlyTotal;
        }

        var monthlyTotalDoc = Mapper.Map(categoryMonthlyTotal);

        if (monthlyTotalDoc is null)
            return Result.Fail<CategoryMonthlyTotal?>("Could not map the AccountMonthlyTotal to its Document");

        await AddDocumentAsync(monthlyTotalDoc, cancellationToken);

        var newMonthlyTotal = Mapper.Map(monthlyTotalDoc);

        return Result.Ok<CategoryMonthlyTotal?>(newMonthlyTotal);
    }

    public async Task<Result<CategoryMonthlyTotal?>> GetAsync(
        string userId,
        string categoryId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(categoryId, out var categoryObjectId) ||
            categoryObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid CategoryId: `{categoryId}`");

        var monthlyTotalDoc = await FirstOrDefaultDocumentAsync(a =>
                a.UserId.Equals(userId) &&
                a.CategoryId.Equals(categoryId) &&
                a.Year == year &&
                a.Month == month,
            cancellationToken);

        if (monthlyTotalDoc is null)
            return Result.Ok<CategoryMonthlyTotal?>(null);

        var accountMonthlyTotal = Mapper.Map(monthlyTotalDoc);

        if (accountMonthlyTotal is null)
            return Result.Fail<CategoryMonthlyTotal?>("Could not map the AccountMonthlyTotal Document to its Entity");

        return accountMonthlyTotal;
    }

    public async Task<Result<IEnumerable<CategoryMonthlyTotal>>> ListAsync(string userId, string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(categoryId, out var categoryObjectId) ||
            categoryObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{categoryId}`");

        var monthlyTotalDocs = await FindDocumentsAsync(a =>
            a.UserId.Equals(userId) &&
            a.CategoryId.Equals(categoryObjectId), cancellationToken);

        var accountMonthlyTotals = Mapper.Map(monthlyTotalDocs);

        return Result.Ok(accountMonthlyTotals);
    }

    public async Task<Result<IEnumerable<CategoryMonthlyTotal>>> ListAsync(string userId, string categoryId, int year, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(categoryId, out var categoryObjectId) ||
            categoryObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{categoryId}`");

        var monthlyTotalDocs = await FindDocumentsAsync(a =>
            a.UserId.Equals(userId) &&
            a.CategoryId.Equals(categoryObjectId) &&
            a.Year.Equals(year), cancellationToken);

        var accountMonthlyTotals = Mapper.Map(monthlyTotalDocs);

        return Result.Ok(accountMonthlyTotals);
    }

    public async Task<Result<IEnumerable<CategoryMonthlyTotal>>> ListAsync(string userId, string categoryId, int year, int month, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail("UserId cannot be null or empty");

        if (!ObjectId.TryParse(categoryId, out var categoryObjectId) ||
            categoryObjectId.Equals(ObjectId.Empty))
            return Result.Fail($"Invalid AccountId: `{categoryId}`");

        var monthlyTotalDocs = await FindDocumentsAsync(a =>
            a.UserId.Equals(userId) &&
            a.CategoryId.Equals(categoryObjectId) &&
            a.Year.Equals(year) &&
            a.Month.Equals(month)
            , cancellationToken);

        var accountMonthlyTotals = Mapper.Map(monthlyTotalDocs);

        return Result.Ok(accountMonthlyTotals);
    }


    public async Task<Result<bool>> UpdateAsync(
        CategoryMonthlyTotal accountMonthlyTotal,
        CancellationToken cancellationToken = default)
    {
        var monthlyTotalDoc = Mapper.Map(accountMonthlyTotal);

        if (monthlyTotalDoc is null)
            return Result.Fail("Could not map the AccountMonthlyTotal to its Document");

        return (await UpdateDocumentAsync(monthlyTotalDoc, cancellationToken)) > 0;
    }
}