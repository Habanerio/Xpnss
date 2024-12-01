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

    public static TSection? GetSettings<TSection>(string settingsKey) where TSection : AppSettingsSection
    {
        if (string.IsNullOrWhiteSpace(settingsKey))
            throw new ArgumentNullException(nameof(settingsKey));

        var section = AppSettings.GetSection(settingsKey).Get<TSection>();

        if (section is null)
            throw new ArgumentNullException($"The settings for {settingsKey} could not be found.");

        section.Environment = EnvironmentName;

        return section;
    }

    /// <summary>
    /// Gets the settings based on the type's name (typeof(DbSettings).Name).
    /// </summary>
    /// <typeparam name="TSection"></typeparam>
    /// <returns></returns>
    public static TSection? GetSettings<TSection>() where TSection : AppSettingsSection
    {
        var settingsKey = typeof(TSection).Name;

        return GetSettings<TSection>(settingsKey);
    }

    public static string AspNetCoreEnvironmentVariableKey => "ASPNETCORE_ENVIRONMENT";

    public static string EnvironmentName
    {
        get
        {
            var environmentName = Environment.GetEnvironmentVariable(
                AspNetCoreEnvironmentVariableKey,
                EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(environmentName))
            {
                environmentName = Environment.GetEnvironmentVariable(
                    AspNetCoreEnvironmentVariableKey,
                    EnvironmentVariableTarget.User);

                if (string.IsNullOrWhiteSpace(environmentName))
                {
                    environmentName = Environment.GetEnvironmentVariable(
                        AspNetCoreEnvironmentVariableKey,
                        EnvironmentVariableTarget.Process);
                }
            }

            return environmentName;
        }
    }
}