using Microsoft.Extensions.Options;

namespace Habanerio.Core.Configurations;

public abstract class AppSettingsSection
{
    public TSettings? Get<TSettings>()
    {
        return AppSettingsHelper.GetSettings<TSettings>();
    }

    public IOptions<TSettings> AsIOptions<TSettings>() where TSettings : class
    {
        return (IOptions<TSettings>)Options.Create(this);
    }
}