using System.Text.Json;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Documents;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.Totals.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Totals.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.Transactions.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.UserProfiles.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Tests.Functional.AppApis;

public class BaseFunctionalApisTests : IDisposable
{
    protected const string API_VERSION = "v1";

    protected const int DEFAULT_PAGE_NO = 1;
    protected const int DEFAULT_PAGE_SIZE = 25;

    protected const string TEST_USER_EMAIL = "test-user@test.com";

    protected readonly HttpClient XpnssApiClient;

    protected readonly IConfiguration Config;

    protected const string ENDPOINTS_ACCOUNTS_CREATE_ACCOUNT = "/api/v1/users/{userId}/accounts";
    protected const string ENDPOINTS_ACCOUNTS_GET_ACCOUNT = "/api/v1/users/{userId}/accounts/{accountId}";
    protected const string ENDPOINTS_ACCOUNTS_GET_ACCOUNTS = "/api/v1/users/{userId}/accounts";
    protected const string ENDPOINTS_ACCOUNTS_UPDATE_ACCOUNT_DETAILS = "/api/v1/users/{userId}/accounts/{accountId}";

    protected const string ENDPOINTS_CATEGORIES_CREATE_CATEGORY = "/api/v1/users/{userId}/categories";
    protected const string ENDPOINTS_CATEGORIES_ADD_SUBCATEGORIES = "/api/v1/users/{userId}/categories/{categoryId}/subcategories";
    protected const string ENDPOINTS_CATEGORIES_GET_CATEGORY = "/api/v1/users/{userId}/categories/{categoryId}";
    protected const string ENDPOINTS_CATEGORIES_GET_CATEGORIES = "/api/v1/users/{userId}/categories";

    protected const string ENDPOINTS_REGISTER = "/api/v1/register";

    protected const string ENDPOINTS_TRANSACTIONS_CREATE_TRANSACTION = "/api/v1/users/{userId}/transactions";
    protected const string ENDPOINTS_TRANSACTIONS_GET_TRANSACTIONS = "/api/v1/users/{userId}/transactions/search";
    protected const string ENDPOINTS_TRANSACTIONS_GET_TRANSACTION = "/api/v1/users/{userId}/transactions/{transactionId}";
    protected const string ENDPOINTS_TRANSACTIONS_COPY_TRANSACTION = "/api/v1/users/{userId}/transactions/{transactionId}/copy";

    protected const string ENDPOINTS_USER_PROFILES_GET_USER_PROFILE = "/api/v1/users/{userId}";

    protected readonly AccountsRepository AccountDocumentsRepository;
    protected readonly CategoriesRepository CategoryDocumentsRepository;
    protected readonly MonthlyTotalsRepository MonthlyTotalDocumentsRepository;
    protected readonly PayerPayeesRepository PayerPayeeDocumentsRepository;
    protected readonly TransactionsRepository TransactionDocumentsRepository;
    protected readonly UserProfilesRepository UserProfileDocumentsRepository;

    protected readonly JsonSerializerOptions JsonSerializationOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly IMongoClient _mongoClient;
    private readonly IMongoDatabase _mongoDb;

    protected static Random RandomGenerator => new();

    protected BaseFunctionalApisTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    {
        XpnssApiClient = factory.CreateClient();

        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var mongoDbSettings = new MongoDbSettings();
        Config.GetSection("XpnssMongoDBSettings").Bind(mongoDbSettings);
        var options = Options.Create(mongoDbSettings);

        _mongoClient = new MongoClient(options.Value.ConnectionString);
        _mongoDb = _mongoClient.GetDatabase(options.Value.DatabaseName);

        var monthlyTotalsLogger = new LoggerFactory().CreateLogger<MonthlyTotalsRepository>();
        var userProfilesRepositoryLogger = new LoggerFactory().CreateLogger<UserProfilesRepository>();

        AccountDocumentsRepository = new AccountsRepository(_mongoDb);
        CategoryDocumentsRepository = new CategoriesRepository(_mongoDb);
        MonthlyTotalDocumentsRepository = new MonthlyTotalsRepository(_mongoDb, monthlyTotalsLogger);
        PayerPayeeDocumentsRepository = new PayerPayeesRepository(_mongoDb);
        TransactionDocumentsRepository = new TransactionsRepository(_mongoDb);
        UserProfileDocumentsRepository = new UserProfilesRepository(_mongoDb, userProfilesRepositoryLogger);

        //TODO: Add Api Key
        //Config = AppConfigSettingsManager.GetConfigs();
        //var apiKey = Config.GetValue<string>("ApiKey");

        //XpnssApiClient.DefaultRequestHeaders.AddDocument("xpnss-api-key", apiKey);
    }

    protected async Task<IEnumerable<MonthlyTotalDocument>> GetMonthlyTotalsAsync(
        ObjectId userId, int? year, int? month)
    {
        var monthlyTotals =
            await MonthlyTotalDocumentsRepository.FindDocumentsAsync(t =>
            t.UserId.Equals(userId) &&
            (!year.HasValue || t.Year == year) &&
            (!month.HasValue || t.Month == month));

        return monthlyTotals;
    }

    protected async Task<CategoryDocument> GetHomeExpensesCategoryAsync()
    {
        var doc = await CategoryDocumentsRepository.FirstOrDefaultDocumentAsync(c =>
            c.Name.Equals("Home"));

        if (doc is null)
            throw new InvalidOperationException("Home Expenses category not found");

        return doc;
    }

    protected async Task<CategoryDocument> GetPersonalExpensesCategoryAsync()
    {
        var doc = await CategoryDocumentsRepository
            .FirstOrDefaultDocumentAsync(c =>
            c.Name.Equals("Personal"));

        if (doc is null)
            throw new InvalidOperationException("Personal Expenses category not found");

        return doc;
    }

    protected async Task<List<PayerPayeeDocument>> GetExistingPayerPayeesAsync()
    {
        var testUserId = await GetTestUserIdAsync();

        var existingPayerPayees =
            (await PayerPayeeDocumentsRepository
                .FindDocumentsAsync(p => p.UserId.Equals(testUserId)))?
            .ToList() ?? [];

        return existingPayerPayees;
    }

    protected async Task<PayerPayeeDocument?> GetRandomPayerPayeeAsync()
    {
        var existingPayerPayees = await GetExistingPayerPayeesAsync();

        // Check for a random PayerPayee. If one can not be provided to you, then create a new random one.
        var existingPayerPayee = existingPayerPayees.Any() ?
            existingPayerPayees
                .ToList()[new Random()
                    .Next(0, existingPayerPayees.Count - 1)] :
            default;

        return existingPayerPayee;
    }

    protected static DateTime GetRandomPastDate =>
        DateTime.Now.AddDays(-(RandomGenerator.Next(1, 180)));

    protected async Task<string> GetTestUserIdAsync()
    {
        return (await GetTestUserObjectIdAsync()).ToString() ?? string.Empty;
    }

    protected async Task<ObjectId> GetTestUserObjectIdAsync()
    {
        var user = await UserProfileDocumentsRepository
            .FirstOrDefaultDocumentAsync(u => u.Email.Equals(TEST_USER_EMAIL));

        return user?.Id ?? ObjectId.Empty;
    }

    public void Dispose()
    {
        XpnssApiClient.Dispose();
        _mongoClient.Dispose();
    }
}