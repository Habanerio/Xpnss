using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Habanerio.Xpnss.Tests.Functional.AppApis;

public class BaseFunctionalApisTests
{
    private readonly WebApplicationFactory<Apis.App.AppApis.Program> _factory;

    protected readonly HttpClient HttpClient;
    protected readonly IConfiguration Config;

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

        //Config = AppConfigSettingsManager.GetConfigs();
        //var apiKey = Config.GetValue<string>("ApiKey");

        //HttpClient.DefaultRequestHeaders.AddDocument("xpnss-api-key", apiKey);
    }
}