using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Habanerio.Xpnss.Tests.Functional.AppApis;

public class BaseFunctionalApisTests
{
    private readonly WebApplicationFactory<Apis.App.AppApis.Program> _factory;

    protected readonly HttpClient HttpClient;

    protected readonly IConfiguration Config;

    protected readonly IAccountsRepository AccountsRepository;

    protected const string API_VERSION = "v1";

    protected const string USER_ID = "test-user-id";

    protected const int DEFAULT_PAGE_NO = 1;
    protected const int DEFAULT_PAGE_SIZE = 25;

    protected BaseFunctionalApisTests(WebApplicationFactory<Apis.App.AppApis.Program> factory)
    {
        _factory = factory;
        //_factory.WithWebHostBuilder(builder =>
        //{
        //    builder.UseEnvironment("Test");
        //});

        HttpClient = _factory.CreateClient();

        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var mongoDbSettings = new MongoDbSettings();
        Config.GetSection("XpnssMongoDBSettings").Bind(mongoDbSettings);
        var options = Options.Create(mongoDbSettings);

        AccountsRepository = new AccountsRepository(options);

        //Config = AppConfigSettingsManager.GetConfigs();
        //var apiKey = Config.GetValue<string>("ApiKey");

        //HttpClient.DefaultRequestHeaders.AddDocument("xpnss-api-key", apiKey);
    }

    protected async Task<IEnumerable<AccountDocument>> GetAccountDocsAsync()
    {
        var accountsResults = await AccountsRepository.ListAsync(USER_ID, CancellationToken.None);

        if (accountsResults.IsFailed)
            throw new Exception($"Failed to get accounts: {accountsResults.Errors[0].Message}");

        var accounts = accountsResults.Value;

        return accounts;
    }
}