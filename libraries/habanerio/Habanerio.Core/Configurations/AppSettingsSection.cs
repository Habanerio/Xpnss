using Microsoft.Extensions.Options;

namespace Habanerio.Core.Configurations;

public abstract class AppSettingsSection
{
    public string Environment { get; set; }

    public TSection? Get<TSection>() where TSection : AppSettingsSection
    {
        return AppSettingsHelper.GetSettings<TSection>();
    }

    public IOptions<TSection> AsIOptions<TSection>() where TSection : AppSettingsSection
    {
        return (IOptions<TSection>)Options.Create(this);
    }
}