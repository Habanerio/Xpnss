using System.Text.Json;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Documents;
using Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.Transactions.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.UserProfiles.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Tests.Functional.AppApis;

public class BaseFunctionalApisTests
{
    protected const string API_VERSION = "v1";

    protected const int DEFAULT_PAGE_NO = 1;
    protected const int DEFAULT_PAGE_SIZE = 25;

    protected const string TEST_USER_EMAIL = "test-user@test.com";

    protected readonly HttpClient HttpClient;

    protected readonly IConfiguration Config;

    protected readonly AccountsRepository AccountDocumentsRepository;
    protected readonly CategoriesRepository CategoryDocumentsRepository;
    protected readonly MonthlyTotalsRepository MonthlyTotalDocumentsRepository;
    protected readonly PayerPayeesRepository PayerPayeeDocumentsRepository;
    protected readonly TransactionsRepository TransactionDocumentsRepository;
    protected readonly UserProfilesRepository UserProfileDocumentsRepository;

    protected readonly JsonSerializerOptions JsonSerializationOptions = new() { PropertyNameCaseInsensitive = true };

    protected static Random RandomGenerator => new();

    protected BaseFunctionalApisTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    {
        HttpClient = factory.CreateClient();

        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var mongoDbSettings = new MongoDbSettings();
        Config.GetSection("XpnssMongoDBSettings").Bind(mongoDbSettings);
        var options = Options.Create(mongoDbSettings);

        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(options.Value.DatabaseName);

        var monthlyTotalsLogger = new LoggerFactory().CreateLogger<MonthlyTotalsRepository>();
        var userProfilesRepositoryLogger = new LoggerFactory().CreateLogger<UserProfilesRepository>();

        AccountDocumentsRepository = new AccountsRepository(mongoDb);
        CategoryDocumentsRepository = new CategoriesRepository(mongoDb);
        MonthlyTotalDocumentsRepository = new MonthlyTotalsRepository(mongoDb, monthlyTotalsLogger);
        PayerPayeeDocumentsRepository = new PayerPayeesRepository(mongoDb);
        TransactionDocumentsRepository = new TransactionsRepository(mongoDb);
        UserProfileDocumentsRepository = new UserProfilesRepository(mongoDb, userProfilesRepositoryLogger);

        //TODO: Add Api Key
        //Config = AppConfigSettingsManager.GetConfigs();
        //var apiKey = Config.GetValue<string>("ApiKey");

        //HttpClient.DefaultRequestHeaders.AddDocument("xpnss-api-key", apiKey);
    }

    protected async Task<IEnumerable<MonthlyTotalDocument>> GetMonthlyTotalsAsync(ObjectId userId, int? year, int? month)
    {
        var monthlyTotals = await MonthlyTotalDocumentsRepository.FindDocumentsAsync(t =>
            t.UserId.Equals(userId) &&
            (!year.HasValue || t.Year == year) &&
            (!month.HasValue || t.Month == month));

        return monthlyTotals;
    }

    protected static DateTime GetRandomPastDate => DateTime.Now.AddDays(-(RandomGenerator.Next(1, 365)));

    protected async Task<string> GetTestUserIdAsync()
    {
        return (await GetTestUserObjectIdAsync()).ToString() ?? string.Empty;
    }

    protected async Task<ObjectId> GetTestUserObjectIdAsync()
    {
        var user = await UserProfileDocumentsRepository.FirstOrDefaultDocumentAsync(u => u.Email.Equals(TEST_USER_EMAIL));

        return user?.Id ?? ObjectId.Empty;
    }
}