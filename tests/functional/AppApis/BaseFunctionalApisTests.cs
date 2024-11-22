using System.Text.Json;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Repositories;
using Habanerio.Xpnss.Merchants.Infrastructure;
using Habanerio.Xpnss.Transactions.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Tests.Functional.AppApis;

public class BaseFunctionalApisTests
{
    private readonly WebApplicationFactory<Apis.App.AppApis.Program> _factory;

    protected readonly HttpClient HttpClient;

    protected readonly IConfiguration Config;

    protected readonly AccountsRepository AccountDocumentsRepository;
    protected readonly CategoriesRepository CategoryDocumentsRepository;
    protected readonly MerchantsRepository MerchantDocumentsRepository;
    protected readonly TransactionsRepository TransactionDocumentsRepository;

    protected readonly JsonSerializerOptions JsonSerializationOptions = new() { PropertyNameCaseInsensitive = true };

    protected const string API_VERSION = "v1";

    protected const string USER_ID = "test-user-id";

    protected const int DEFAULT_PAGE_NO = 1;
    protected const int DEFAULT_PAGE_SIZE = 25;

    protected BaseFunctionalApisTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    {
        _factory = factory;

        HttpClient = _factory.CreateClient();

        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var mongoDbSettings = new MongoDbSettings();
        Config.GetSection("XpnssMongoDBSettings").Bind(mongoDbSettings);
        var options = Options.Create(mongoDbSettings);

        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(options.Value.DatabaseName);

        AccountDocumentsRepository = new AccountsRepository(mongoDb);
        CategoryDocumentsRepository = new CategoriesRepository(mongoDb);
        MerchantDocumentsRepository = new MerchantsRepository(options);
        TransactionDocumentsRepository = new TransactionsRepository(options);

        //TODO: Add Api Key
        //Config = AppConfigSettingsManager.GetConfigs();
        //var apiKey = Config.GetValue<string>("ApiKey");

        //HttpClient.DefaultRequestHeaders.AddDocument("xpnss-api-key", apiKey);
    }
}