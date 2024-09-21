using Microsoft.Extensions.Configuration;

namespace Habanerio.Core.Configurations;

public static class AppSettingsHelper
{
    private static IConfiguration? _appSettings;

    /// <summary>
    /// Gets the entirety of the appSettings.json file for the current environment that is set in 'ASPNETCORE_ENVIRONMENT'.
    /// </summary>
    public static IConfiguration AppSettings
    {
        get
        {
            if (_appSettings is null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appSettings.json", true, true)
                    .AddJsonFile($"appSettings.{EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables();
                _appSettings = builder.Build();
            }

            return _appSettings;
        }
    }

    public static T? GetSettings<T>(string settingsKey)
    {
        if (string.IsNullOrWhiteSpace(settingsKey))
            throw new ArgumentNullException(nameof(settingsKey));

        var instance = AppSettings.GetSection(settingsKey).Get<T>();

        return instance;
    }

    /// <summary>
    /// Gets the settings based on the type's name (typeof(DbSettings).Name).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? GetSettings<T>()
    {
        var tName = typeof(T).Name;

        return GetSettings<T>(tName);
    }

    public static string AspNetCoreEnvironmentVariableKey => "ASPNETCORE_ENVIRONMENT";

    public static string EnvironmentName
    {
        get
        {
            var environmentName = Environment.GetEnvironmentVariable(AspNetCoreEnvironmentVariableKey, EnvironmentVariableTarget.Machine);
            if (string.IsNullOrWhiteSpace(environmentName))
            {
                environmentName = Environment.GetEnvironmentVariable(AspNetCoreEnvironmentVariableKey, EnvironmentVariableTarget.User);
                if (string.IsNullOrWhiteSpace(environmentName))
                {
                    environmentName = Environment.GetEnvironmentVariable(AspNetCoreEnvironmentVariableKey, EnvironmentVariableTarget.Process);
                }
            }

            return environmentName;
        }
    }
}