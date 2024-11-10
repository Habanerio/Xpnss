using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Infrastructure.Documents;
using Habanerio.Xpnss.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Habanerio.Xpnss.Tests.Functional.AppApis;

public class BaseFunctionalApisTests
{
    private readonly WebApplicationFactory<Apis.App.AppApis.Program> _factory;

    protected readonly HttpClient HttpClient;

    protected readonly IConfiguration Config;

    protected readonly MongoDbRepository<AccountDocument> AccountDocumentsRepository;
    protected readonly MongoDbRepository<CategoryDocument> CategoryDocumentsRepository;
    protected readonly MongoDbRepository<MerchantDocument> MerchantDocumentsRepository;
    protected readonly MongoDbRepository<TransactionDocument> TransactionDocumentsRepository;

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

        AccountDocumentsRepository = new AccountsRepository(options);
        CategoryDocumentsRepository = new CategoriesRepository(options);
        MerchantDocumentsRepository = new MerchantsRepository(options);
        TransactionDocumentsRepository = new TransactionsRepository(options);

        //TODO: Add Api Key
        //Config = AppConfigSettingsManager.GetConfigs();
        //var apiKey = Config.GetValue<string>("ApiKey");

        //HttpClient.DefaultRequestHeaders.AddDocument("xpnss-api-key", apiKey);
    }
}